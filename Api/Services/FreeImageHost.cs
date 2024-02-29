using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Web.Http;
using static System.Net.WebRequestMethods;

namespace Api.Services;

public interface IImageHost
{
    public Task<RemoteImageAction> Send(byte[] imageBytes, string filename = null);
    public Task<RemoteImageAction[]> SendAll((byte[] bytes, string filename)[] allImages);

}

public class FreeImageHost : IImageHost
{
    private readonly Uri _apiUri;
    private readonly ILogger<FreeImageHost> _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="logger"></param>
    /// <exception cref="UriFormatException">Se <paramref name="apiKey"/> for irregular</exception>
    public FreeImageHost(string apiKey, ILogger<FreeImageHost> logger)
    {
        var api = Values.Api.RequestFreeImageHostUrl.AbsolutePath + $"?key={apiKey}";
        _apiUri = new Uri(api);
        _logger = logger;
    }




    /// <summary>
    /// Extrai um <see cref="RemoteImageResponseDTO"/> de <paramref name="imageJson"/>
    /// </summary>
    /// <param name="imageJson"></param>
    /// <returns></returns>
    /// <exception cref="UriFormatException"> Valor encontrado no <paramref name="imageJson"/> para a url é inválido </exception>
    /// <exception cref="ArgumentNullException"> Nó do <paramref name="imageJson"/> para as urls não encontrado </exception>
    private RemoteImageResponseDTO? ExtractResponseDTO(string imageJson)
    {

        var json = JObject.Parse(imageJson);
        var fullImageUrl = json.SelectToken("image/url").Value<string>();
        string mediumImageUrl = json.SelectToken("image.medium.url").Value<string>();
        string thumbImageUrl = json.SelectToken("image.thumb.url").Value<string>();

        return new RemoteImageResponseDTO
        {
            FullImage = new Uri(fullImageUrl),
            Medium = new Uri(mediumImageUrl),
            Thumb = new Uri(thumbImageUrl)
        };

    }

    public async Task<RemoteImageAction> Send(byte[] imageBytes, string filename)
    {
        var client = new HttpClient();


        using var request = new HttpRequestMessage(HttpMethod.Post, Values.Api.RequestFreeImageHostUrl);
        using (var content = new MultipartFormDataContent())
        using (var stream = new MemoryStream(imageBytes))
        {
            content.Add(new StreamContent(stream), "files", filename);

            request.Content = content;
            var result = await client.SendAsync(request);
            if (!result.IsSuccessStatusCode)
            {
                _logger.LogError("Resposta da API retornou erro " + result.StatusCode);
                return new RemoteImageAction
                {
                    StatusCode = result.StatusCode
                };
            }
            
            
            try
            {
                return new RemoteImageAction
                {
                    Result = ExtractResponseDTO(await result.Content.ReadAsStringAsync()),
                    StatusCode = HttpStatusCode.Created
                };


            }

            //Exceptions...
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Resposta da API em formato desconhecido");
                return new RemoteImageAction { StatusCode = HttpStatusCode.InternalServerError};

            }
            catch (UriFormatException ex)
            {
                _logger.LogError(ex, "Resposta da API em formato desconhecido");
                return new RemoteImageAction { StatusCode = HttpStatusCode.InternalServerError };
            }

        }
    }

    public async Task<RemoteImageAction[]> SendAll((byte[] bytes, string filename)[] allImages)
    {
        var tasks = allImages.Select(x => Send(x.bytes, x.filename));
        return await Task.WhenAll(tasks);
    }
}


public record RemoteImageResponseDTO
{
    public Uri FullImage { get; init; }
    public Uri? Thumb { get; init; }
    public Uri? Medium { get; init; }

}

public record RemoteImageAction
{
    public HttpStatusCode StatusCode { get; init; }
    public RemoteImageResponseDTO? Result { get; init; }
}