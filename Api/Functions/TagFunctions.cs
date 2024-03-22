using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Web;
using Api.Database;
using Api.Models;
using AutoMapper;
using FluentValidation;
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

public class TagFunctions : CrudBase<Tag, string, TagRequestDTO, TagResponseDTO>
{
    public TagFunctions(ILogger<Tag> logger, IMapper mapper, ITagRepository repository, IDescribes describes, IValidator<TagRequestDTO> validator) : base(logger, mapper, repository, describes, validator)
    {
    }

    [Function(nameof(GetById))]
    [OpenApiOperation(Values.Api.TagGetById, Description = "Obtem uma única tag correspondente ao nome")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MimeMapping.KnownMimeTypes.Json, typeof(TagResponseDTO))]
    public async ValueTask<IActionResult> GetByName([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Values.Api.TagGetById)]HttpRequestData req, string id)
        => await base.GetById(req, id.ToLower());

    [Function(nameof(Get))]
    [OpenApiOperation(Values.Api.TagGet, Description = "Obtém uma consulta de tags.")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MimeMapping.KnownMimeTypes.Json, typeof(TagResponseDTO[]))]
    public async new ValueTask<IActionResult> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Values.Api.TagGet)]HttpRequestData req)
        => await base.Get(req, Values.Api.TagGetQuery);


    [Function(nameof(Create))]
    [OpenApiOperation(Values.Api.TagCreate, Description = "Adiciona uma tag")]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MimeMapping.KnownMimeTypes.Json, typeof(TagResponseDTO))]
    public async ValueTask<IActionResult> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Values.Api.TagCreate)]HttpRequestData req)
        => await base.Add(req);

    [Function(nameof(Rename)),]
    [OpenApiOperation(Values.Api.TagRename, Description = "Renomeia uma tag")]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MimeMapping.KnownMimeTypes.Json, typeof(TagResponseDTO))]
    public async ValueTask<IActionResult> Rename([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = Values.Api.TagRename)] HttpRequestData req, string name)
        => await base.Put(req, name.ToLower());

    [Function(nameof(Delete))]
    [OpenApiOperation(Values.Api.TagDelete, Description = "Adivinha? Remove uma tag, mas também limpa as relações com as outras entidades")]
    [OpenApiResponseWithoutBody(HttpStatusCode.OK, Description = "Sucesso")]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, KnownMimeTypes.Json, typeof(string))]
    public async ValueTask<IActionResult> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = Values.Api.TagDelete)] HttpRequestData req, string name)
        => await base.Delete(name.ToLower());
}

