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
using Newtonsoft.Json;
using Shared;
using Shared.Api;
using Shared.Models;

namespace Api;

public class TagFunctions
{
    private readonly ILogger<TagFunctions> _logger;
    private readonly IMapper _mapper;
    private readonly ITagRepository _repository;
    private readonly IDescribes _describes;
    private readonly IValidator<TagRequestDTO> _validator;

    public TagFunctions(ILogger<TagFunctions> logger, IMapper mapper, ITagRepository repository, IDescribes describes, IValidator<TagRequestDTO> validator)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
        _describes = describes;
        _validator = validator;
    }

    [Function(Values.Api.TagGetById)]
    [OpenApiOperation(Values.Api.TagGetById, Description = "Obtém uma única tag correspondente ao nome")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MimeMapping.KnownMimeTypes.Json, typeof(TagResponseDTO))]
    public async ValueTask<IActionResult> GetById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{Values.Api.TagGetById}/{{name}}")] HttpRequestData req, string name)
    {
        var tag = await _repository.GetById(name);
        if (tag == null)
            return new NotFoundObjectResult(name);
        var response = _mapper.Map<Tag, TagResponseDTO>(tag);
        return new JsonResult(response)
        {
            StatusCode = StatusCodes.Status200OK
        };
    }

    [Function(Values.Api.TagGet)]
    [OpenApiOperation(Values.Api.TagGet, Description = "Obtém uma consulta de tags.")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MimeMapping.KnownMimeTypes.Json, typeof(TagResponseDTO[]))]
    public IActionResult Get([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        var encodedQuery = req.Query[Values.Api.TagGetQuery];
        if (encodedQuery == null)
            return new BadRequestObjectResult(_describes.QueryMissing());
        var query = HttpUtility.UrlDecode(encodedQuery);
        
        
        EntityQuery entityQuery;
        try
        {
            entityQuery = JsonConvert.DeserializeObject<EntityQuery>(query);
        }
        catch (System.Exception e)
        {
            _logger.LogError("Erro ao converter json de query para objeto. Retornando 400");
            _logger.LogError(e.Message);
            return new JsonResult(null)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        var final = _repository.GetWithQuery(entityQuery);
        return new JsonResult(final)
        {
            StatusCode = StatusCodes.Status200OK
        };
    }

    [Function(Values.Api.TagAdd)]
    [OpenApiOperation(Values.Api.TagAdd, Description = "Adiciona uma tag.")]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MimeMapping.KnownMimeTypes.Json, typeof(TagResponseDTO))]
    public async Task<IActionResult> Add([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {

        //var dto = await req.ReadFromJsonAsync<TagRequestDTO>();
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var dto = JsonConvert.DeserializeObject<TagRequestDTO>(body);

        if (dto == null)
            return new BadRequestObjectResult(_describes.BadBodyRequest());

        var results = await _validator.ValidateAsync(dto);
        var modelState = ModelState.FromValidationResult(results);
        if (!modelState.IsValid)
            return new UnprocessableEntityObjectResult(modelState.GroupByProperty());

        var entity = _mapper.Map<TagRequestDTO, Tag>(dto);
        entity.Name = entity.Name.ToLower();
        await _repository.Add(entity);

        var responseDTO = _mapper.Map<Tag, TagResponseDTO>(entity);

        return new JsonResult(responseDTO)
        {
            StatusCode = StatusCodes.Status201Created
        };
    }
}
