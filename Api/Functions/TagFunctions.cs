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

public class TagFunctions : FunctionBase
{

    protected readonly IMapper _mapper;
    protected readonly ITagRepository _repository;
    protected readonly IValidator<TagRequestDTO> _validator;

    public TagFunctions(IMapper mapper, ITagRepository repository, IValidator<TagRequestDTO> validator, ILogger<TagFunctions> logger, IDescribes describes) : base(logger, describes)
    {
        _mapper = mapper;
        _repository = repository;
        _validator = validator;
    }

    [Function(nameof(GetByName))]
    [OpenApiOperation(Values.Api.TagGetByName, Description = "Obtem uma única tag correspondente ao nome")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MimeMapping.KnownMimeTypes.Json, typeof(TagResponseDTO))]
    public async ValueTask<IActionResult> GetByName([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Values.Api.TagGetByName)] HttpRequestData req, string name)
    {
        var tag = await _repository.GetByName(name);
        if (tag == null)
            return new NotFoundObjectResult(name);
        var response = _mapper.Map<Tag, TagResponseDTO>(tag);
        return new JsonResult(response)
        {
            StatusCode = StatusCodes.Status200OK
        };
    }


    [Function(nameof(Get))]
    [OpenApiOperation(Values.Api.TagGet, Description = "Obtém uma consulta de tags.")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MimeMapping.KnownMimeTypes.Json, typeof(TagResponseDTO[]))]
    public async new ValueTask<IActionResult> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Values.Api.TagGet)] HttpRequestData req)
    {
        var entityQueryProcessed = GetEntityQuery(req);
        if (entityQueryProcessed.IsFailure)
        {
            return new JsonResult(entityQueryProcessed.Error)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
        var final = await _repository.GetWithQuery(entityQueryProcessed.Data!).ToDynamicArrayAsync();
        return new JsonResult(final)
        {
            StatusCode = StatusCodes.Status200OK
        };

    }


    [Function(nameof(Create))]
    [OpenApiOperation(Values.Api.TagCreate, Description = "Adiciona uma tag")]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MimeMapping.KnownMimeTypes.Json, typeof(TagResponseDTO))]
    public async ValueTask<IActionResult> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Values.Api.TagCreate)] HttpRequestData req)
    {
        var dto = await GetJsonFromBody<TagRequestDTO>(req);

        if (dto.IsFailure)
            return new JsonResult(dto.Error)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };


        var results = await _validator.ValidateAsync(dto.Data, opt => opt.IncludeRuleSets("default", TagValidator.RuleSetAdd));
        var modelState = ModelState.FromValidationResult(results);
        if (!modelState.IsValid)
            return new JsonResult(modelState.GroupByProperty())
            {
                StatusCode = StatusCodes.Status422UnprocessableEntity
            };

        var entity = _mapper.Map<TagRequestDTO, Tag>(dto.Data);
        await _repository.Add(entity);

        var responseDTO = _mapper.Map<Tag, TagResponseDTO>(entity);

        return new JsonResult(responseDTO)
        {
            StatusCode = StatusCodes.Status201Created
        };
    }


    [Function(nameof(Rename)),]
    [OpenApiOperation(Values.Api.TagRename, Description = "Renomeia uma tag")]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MimeMapping.KnownMimeTypes.Json, typeof(TagResponseDTO))]
    public async ValueTask<IActionResult> Rename([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = Values.Api.TagRename)] HttpRequestData req, string name)
    {
        var dto = await GetJsonFromBody<TagRequestDTO>(req);

        if (dto.IsFailure)
            return new JsonResult(_describes.BadBodyRequest())
            {
                StatusCode = StatusCodes.Status400BadRequest
            };



        var results = await _validator.ValidateAsync(dto.Data, opt => opt.IncludeRuleSets("default", TagValidator.RuleSetUpdate));
        var modelState = ModelState.FromValidationResult(results);
        if (!modelState.IsValid)
            return new JsonResult(modelState.GroupByProperty())
            {
                StatusCode = StatusCodes.Status422UnprocessableEntity
            };

        var entity = _mapper.Map<TagRequestDTO, Tag>(dto.Data);
        var result = await _repository.Rename(name, entity.Name);

        if (!result)
            return new JsonResult(null)
            {
                StatusCode = StatusCodes.Status404NotFound
            };

        var responseDTO = _mapper.Map<Tag, TagResponseDTO>(entity);

        return new JsonResult(responseDTO)
        {
            StatusCode = StatusCodes.Status200OK
        };
    }

    [Function(nameof(Delete))]
    [OpenApiOperation(Values.Api.TagDelete, Description = "Adivinha? Remove uma tag, mas também limpa as relações com as outras entidades")]
    [OpenApiResponseWithoutBody(HttpStatusCode.OK, Description = "Sucesso")]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, KnownMimeTypes.Json, typeof(string))]
    public async ValueTask<IActionResult> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = Values.Api.TagDelete)] HttpRequestData req, string name)
    {

        var result = await _repository.DeleteByName(name);

        if (!result)
            return new JsonResult(_describes.KeyNotFound(name))
            {
                StatusCode = StatusCodes.Status404NotFound
            };

        return new JsonResult(null)
        {
            StatusCode = StatusCodes.Status200OK
        };
    }
}

