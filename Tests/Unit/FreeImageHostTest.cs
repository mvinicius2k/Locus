using Api.Services;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MimeMapping;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Fixtures;
using Tests.Helpers;

namespace Tests.Unit;

public class FreeImageHostTest : IClassFixture<MinimalLogger>
{
    const string ApiKey = "6d207e02198a847aa98d0a2a901485a5";

    private readonly FreeImageHost _freeImageHost;
    private readonly MinimalLogger _minimalLogger;

    public FreeImageHostTest(MinimalLogger logger)
    {
        var mock = new Mock<FreeImageHost>();
        _minimalLogger = logger;

        var serviceLogger = _minimalLogger.GetInstance<FreeImageHost>();
        _freeImageHost = new FreeImageHost(ApiKey, serviceLogger);
    }


    //public Task Upload_5_Supported_Images_Should_Be_Sucess(IEnumerable<byte[]> imageList)
    //{
    //    _freeImageHost.SendAll
    //}

    [Theory, MemberData(nameof(SupportedImagesParams))]
    public async Task Upload_Supported_Image_Should_Be_Sucess(byte[] imageBytes, string filename)
    {
        var response = await _freeImageHost.Send(imageBytes, filename);
        
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        response.Result.Should().NotBeNull();

    }


    #region MemberData

    public static IEnumerable<object[]> SupportedImagesParams()
    {
        var random = new Random(10);
        var allImages = ImageUtils.GetRandonly(random);
        var uniqueImages = allImages.DistinctBy(im => MimeUtility.GetMimeMapping(im.Extension)).Select(im =>
        {
            using (var fs = im.OpenRead())
            using (var ms = new MemoryStream())
            {
                ms.CopyTo(ms);
                (FileInfo fileInfo, byte[] bytes) tuple = (im, ms.ToArray());
                return tuple;
            }
                
            
        });

        foreach (var item in uniqueImages)
            yield return new object[] { item.bytes, item.fileInfo };



    }

    #endregion

}
