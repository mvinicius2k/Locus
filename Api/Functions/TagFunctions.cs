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
    protected TagFunctions(ILogger<Tag> logger, IMapper mapper, ITagRepository repository, IDescribes describes, IValidator<TagRequestDTO> validator) : base(logger, mapper, repository, describes, validator)
    {
    }

    [Function(Values.Api.TagGetById)]
    [OpenApiOperation(Values.Api.TagGetById, Description = "Obtém uma única tag correspondente ao nome")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MimeMapping.KnownMimeTypes.Json, typeof(TagResponseDTO))]
    public async new ValueTask<IActionResult> GetById([HttpTrigger(AuthorizationLevel.Anonymous, "get")]HttpRequestData req, string id)
        => await base.GetById(req, id.ToLower());

    [Function(Values.Api.TagGet)]
    [OpenApiOperation(Values.Api.TagGet, Description = "Obtém uma consulta de tags.")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MimeMapping.KnownMimeTypes.Json, typeof(TagResponseDTO[]))]
    public async new ValueTask<IActionResult> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get")]HttpRequestData req, string queryKey)
        => await base.Get(req, queryKey);


    [Function(Values.Api.TagAdd)]
    [OpenApiOperation(Values.Api.TagAdd, Description = "Adiciona uma tag.")]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MimeMapping.KnownMimeTypes.Json, typeof(TagResponseDTO))]
    public async ValueTask<IActionResult> Add([HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequestData req)
        => await base.Add(req);

    [Function(Values.Api.TagRename)]
    [OpenApiOperation(Values.Api.TagRename, Description = "Renomeia uma tag")]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MimeMapping.KnownMimeTypes.Json, typeof(TagResponseDTO))]
    public async ValueTask<IActionResult> Rename([HttpTrigger(AuthorizationLevel.Anonymous, "put")] HttpRequestData req, string name)
        => await base.Put(req, name.ToLower());

    [Function(Values.Api.TagDelete)]
    [OpenApiOperation(Values.Api.TagDelete, Description = "Adivinha? Remove uma tag, mas também limpa as relações com as outras entidades")]
    [OpenApiResponseWithoutBody(HttpStatusCode.OK, Description = "Sucesso")]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, KnownMimeTypes.Json, typeof(string))]
    public async ValueTask<IActionResult> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete")] HttpRequestData req, string name)
        => await base.Delete(name.ToLower());
}
