using System.Net;
using Api.Models;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using MimeMapping;
using Shared;
using Shared.Models;

namespace Api;

public class PostFunctions : FunctionBase<Post, int, PostRequestDTO, PostResponseDTO>
{
    private readonly IPostRepository _repository;

    public PostFunctions(ILogger<PostFunctions> logger, IDescribes describes, IMapper mapper, IValidator<PostRequestDTO> validator, IPostRepository repository) : base(logger, describes, mapper, validator)
    {
        _repository = repository;
    }

    

    [Function("Get-Post-By-Id")]
    [OpenApiOperation(Values.Api.PostGet, Description = "Obtem um único post correspondente ao id")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, MimeMapping.KnownMimeTypes.Json, typeof(PostResponseDTO))]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Values.Api.PostGetById)] HttpRequestData req, int id)
        => await ActionGetByIndex(id, _repository.GetById);

    [Function("Create-Post")]
    [OpenApiOperation(Values.Api.PostCreate, Description = "Cria um post")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, MimeMapping.KnownMimeTypes.Json, typeof(PostResponseDTO))]
    [OpenApiResponseWithBody(HttpStatusCode.UnprocessableEntity, MimeMapping.KnownMimeTypes.Json, typeof(Dictionary<string, List<string>>))]
    public async Task<IActionResult> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Values.Api.PostCreate)] HttpRequestData req)
        => await ActionCreateEntity(req,_repository.Add);

    [Function("Get-Post")]
    [OpenApiOperation(Values.Api.PostGet, Description = "Obtem uma consulta de Posts")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, MimeMapping.KnownMimeTypes.Json, typeof(PostResponseDTO[]))]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Values.Api.PostGet)] HttpRequestData req)
        => await ActionGetByQuery(req, _repository.GetWithQuery);

    [Function("Update-Post"),]
    [OpenApiOperation(Values.Api.PostUpdate, Description = "Atualiza um post")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MimeMapping.KnownMimeTypes.Json, typeof(PostResponseDTO))]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotFound)]
    public async ValueTask<IActionResult> Update([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = Values.Api.PostUpdate)] HttpRequestData req, int id)
        => await ActionUpdate(req, id, _repository.Update, _repository.Exists, ["default", PostValidator.RuleSetUpdate]);

    [Function("Delete-Post")]
    [OpenApiOperation(Values.Api.PostDelete, Description = "Adivinha? Remove um post, mas também limpa as relações com as outras entidades")]
    [OpenApiResponseWithoutBody(HttpStatusCode.OK, Description = "Sucesso")]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, KnownMimeTypes.Json, typeof(string))]
    public async ValueTask<IActionResult> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = Values.Api.PostDelete)] HttpRequestData req, int id)
        => await ActionDelete(id, _repository.Delete, _repository.Exists);

    


}
