using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Security.Claims;
using System.Web;
using Api.Database;
using Api.Models;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeMapping;
using Newtonsoft.Json;
using Shared;
using Shared.Api;
using Shared.Models;

namespace Api;

public class TagFunctions : FunctionBase<Tag, int, TagRequestDTO, TagResponseDTO>
{
    
    private readonly ITagRepository _repository;

    public TagFunctions(ILogger<TagFunctions> logger, IDescribes describes, IMapper mapper, ITagRepository repository, IValidator<TagRequestDTO> validator) : base(logger, describes, mapper, validator)
    {
        _repository = repository;
    }

    [Function("Get-Post-Auth"), Authorize]
    [OpenApiOperation("Get-Post-Auth")]
    public IActionResult GetPost([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req){
        var user = req.FunctionContext.GetHttpContext().User;
        var identifier = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if(identifier == null)
            return new UnauthorizedObjectResult("Nao");
        return new OkObjectResult("sim");

    }
        

    [Function("Get-Tag-By-Name")]
    [OpenApiOperation(Values.Api.TagGetByName, Description = "Obtem uma única tag correspondente ao nome")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MimeMapping.KnownMimeTypes.Json, typeof(TagResponseDTO))]
    public async Task<IActionResult> GetByName([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Values.Api.TagGetByName)] HttpRequestData req, string name)
        => await ActionGetByIndex(name, _repository.GetByName);
    


    [Function("Get-Tag")]
    [OpenApiOperation(Values.Api.TagGet, Description = "Obtém uma consulta de tags.")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MimeMapping.KnownMimeTypes.Json, typeof(TagResponseDTO[]))]
    public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Values.Api.TagGet)] HttpRequestData req)
        => await ActionGetByQuery(req, _repository.GetWithQuery);


    [Function("Create-Tag")]
    [OpenApiOperation(Values.Api.TagCreate, Description = "Adiciona uma tag")]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MimeMapping.KnownMimeTypes.Json, typeof(TagResponseDTO))]
    public async Task<IActionResult> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Values.Api.TagCreate)] HttpRequestData req)
        => await ActionCreateEntity(req, _repository.Add, ["default", TagValidator.RuleSetAdd]);


    [Function("Rename-Tag"),]
    [OpenApiOperation(Values.Api.TagRename, Description = "Renomeia uma tag")]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MimeMapping.KnownMimeTypes.Json, typeof(TagResponseDTO))]
    public async Task<IActionResult> Rename([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = Values.Api.TagRename)] HttpRequestData req, string name)
        => await ActionUpdateByIndex(req, name, _repository.UpdateByName, _repository.ExistsByName);

    [Function("Delete-Tag")]
    [OpenApiOperation(Values.Api.TagDelete, Description = "Adivinha? Remove uma tag, mas também limpa as relações com as outras entidades")]
    [OpenApiResponseWithoutBody(HttpStatusCode.OK, Description = "Sucesso")]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, KnownMimeTypes.Json, typeof(string))]
    public async ValueTask<IActionResult> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = Values.Api.TagDelete)] HttpRequestData req, string name)
        => await ActionDeleteByIndex(name, _repository.DeleteByName, _repository.ExistsByName);
}

