using Api.Services;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MimeMapping;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Fixtures;
using Tests.Helpers;
using Xunit.Abstractions;

namespace Tests.Unit;

public class FreeImageHostTest : IClassFixture<MinimalLogger>
{
    const string ApiKey = "6d207e02198a847aa98d0a2a901485a5";

    private readonly FreeImageHost _freeImageHost;
    private readonly MinimalLogger _minimalLogger;
    private readonly ITestOutputHelper _output;


    public FreeImageHostTest(MinimalLogger logger, ITestOutputHelper output)
    {
        _minimalLogger = logger;
        _output = output;
        var serviceLogger = _minimalLogger.GetInstance<FreeImageHost>();
        _freeImageHost = new FreeImageHost(ApiKey, serviceLogger);
    }

    [Fact]
    public async Task Upload_5_Jpgs_Images_Should_Be_Sucess()
    {
        var random = new Random(100);
        var allImages = ResourceUtils.GetRandomly(random, KnownMimeTypes.Jpg)
            .Take(5)
            .Select(f => (File.ReadAllBytes(f.FullName), f.Name));

        var response = await _freeImageHost.SendAll(allImages.ToArray());

        response.All(r => r.StatusCode == System.Net.HttpStatusCode.Created);
        response.All(r => r.Result != null);

        _output.WriteLine("Resposta:\n" + JsonConvert.SerializeObject(response, Formatting.Indented));
        
    }

    [Theory, MemberData(nameof(SupportedImagesParams))]
    public async Task Upload_Supported_Image_Should_Be_Sucess(string fileFullPath)
    {


        //Avif e nem bmp são suportados
        var imageBytes = File.ReadAllBytes(fileFullPath);
        var response = await _freeImageHost.Send(imageBytes, Path.GetFileName(fileFullPath));
        
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        response.Result.Should().NotBeNull();
        _output.WriteLine("Resposta:");
        _output.WriteLine(JsonConvert.SerializeObject(response));

    }


    #region MemberData

    public static IEnumerable<object[]> SupportedImagesParams()
    {
        var random = new Random(10);
        var allImages = ResourceUtils.GetRandomly(random, FreeImageHost.HostSupportedMimes);
        var uniqueImages = allImages.DistinctBy(im => MimeUtility.GetMimeMapping(im.Extension))
            .Where(im =>  FreeImageHost.HostSupportedMimes.Contains(MimeUtility.GetMimeMapping(im.Name)))
            .Select(im => im.FullName);
        

        foreach (var item in uniqueImages)
            yield return new object[] { item };



    }


    



    #endregion

}
