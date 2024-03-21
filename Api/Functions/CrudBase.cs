using System.Linq.Dynamic.Core;
using System.Web;
using Api.Models;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared;
using Shared.Api;

namespace Api;

public abstract class CrudBase<TEntity, TId, TRequestDTO, TResponseDTO> where TEntity : class, IEntity<TId>
{
    protected readonly ILogger<TEntity> _logger;
    protected readonly IMapper _mapper;
    protected readonly IRepository<TEntity, TId> _repository;
    protected readonly IDescribes _describes;
    protected readonly IValidator<TRequestDTO> _validator;

    public CrudBase(ILogger<TEntity> logger, IMapper mapper, IRepository<TEntity, TId> repository, IDescribes describes, IValidator<TRequestDTO> validator)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
        _describes = describes;
        _validator = validator;
    }

    protected async ValueTask<IActionResult> GetById(HttpRequestData req, TId id)
    {
        var tag = await _repository.GetById(id);
        if (tag == null)
            return new NotFoundObjectResult(id);
        var response = _mapper.Map<TEntity, TResponseDTO>(tag);
        return new JsonResult(response)
        {
            StatusCode = StatusCodes.Status200OK
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="req"></param>
    /// <param name="queryKey">key da query que está dentro da requisiçao</param>
    /// <returns></returns>
    protected async ValueTask<IActionResult> Get(HttpRequestData req, string queryKey)
    {
        var encodedQuery = req.Query[queryKey];
        if (encodedQuery == null)
            return new BadRequestObjectResult(_describes.QueryMissing());
        var query = HttpUtility.UrlDecode(encodedQuery);

        if (query == null)
        {
            var eventId = new EventId(Values.Events.QueryDecode);
            _logger.LogError(eventId, "Erro ao decodificar query");

            return new JsonResult(eventId)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        EntityQuery entityQuery;
        try
        {
            entityQuery = JsonConvert.DeserializeObject<EntityQuery>(query);
        }
        catch (System.Exception e)
        {
            var eventId = new EventId(Values.Events.QueryDecode);
            _logger.LogError(eventId, e, "Erro ao converter json de query para objeto. Retornando 400");
            return new JsonResult(eventId)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        var final = await _repository.GetWithQuery(entityQuery).ToDynamicArrayAsync();
        return new JsonResult(final)
        {
            StatusCode = StatusCodes.Status200OK
        };
    }

    protected async ValueTask<IActionResult> Add(HttpRequestData req, Func<TRequestDTO, TRequestDTO>? requestTransform = null)
    {
        var dto = await GetJsonFromBody(req);

        if (dto == null)
            return new JsonResult(_describes.BadBodyRequest())
            {
                StatusCode = StatusCodes.Status400BadRequest
            };

        if(requestTransform != null)
            dto = requestTransform.Invoke(dto);
        var results = await _validator.ValidateAsync(dto);
        var modelState = ModelState.FromValidationResult(results);
        if (!modelState.IsValid)
            return new JsonResult(modelState.GroupByProperty())
            {
                StatusCode = StatusCodes.Status422UnprocessableEntity
            };

        var entity = _mapper.Map<TRequestDTO, TEntity>(dto);
        await _repository.Add(entity);

        var responseDTO = _mapper.Map<TEntity, TResponseDTO>(entity);

        return new JsonResult(responseDTO)
        {
            StatusCode = StatusCodes.Status201Created
        };
    }

    protected async ValueTask<IActionResult> Put(HttpRequestData req, TId id, Func<TRequestDTO, TRequestDTO>? requestTransform = null)
    {
        var dto = await GetJsonFromBody(req);

        if (dto == null)
            return new JsonResult(_describes.BadBodyRequest())
            {
                StatusCode = StatusCodes.Status400BadRequest
            };

        var results = await _validator.ValidateAsync(dto);
        var modelState = ModelState.FromValidationResult(results);
        if (!modelState.IsValid)
            return new JsonResult(modelState.GroupByProperty())
            {
                StatusCode = StatusCodes.Status422UnprocessableEntity
            };

        if(requestTransform != null)
            dto = requestTransform.Invoke(dto);

        var entity = _mapper.Map<TRequestDTO, TEntity>(dto);
        var result = await _repository.TryUpdate(id, entity);

        if (!result)
            return new JsonResult(null)
            {
                StatusCode = StatusCodes.Status404NotFound
            };

        var responseDTO = _mapper.Map<TEntity, TResponseDTO>(entity);

        return new JsonResult(responseDTO)
        {
            StatusCode = StatusCodes.Status201Created
        };
    }

    protected async ValueTask<IActionResult> Delete(TId id)
    {
        var result = await _repository.TryDelete(id);

        if (!result)
            return new JsonResult(_describes.KeyNotFound(id))
            {
                StatusCode = StatusCodes.Status404NotFound
            };

        return new JsonResult(null)
        {
            StatusCode = StatusCodes.Status200OK
        };
    }


    protected async ValueTask<TRequestDTO?> GetJsonFromBody(HttpRequestData req)
    {
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var dto = JsonConvert.DeserializeObject<TRequestDTO>(body);

        return dto;
    }
}
