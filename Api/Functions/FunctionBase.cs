using System.Linq.Dynamic.Core;
using System.Net;
using System.Web;
using Api.Models;
using AutoMapper;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared;
using Shared.Api;
using Shared.Models;

namespace Api;

public abstract class FunctionBase<TEntity, TId, TDTORequest, TDTOResponse> where TEntity : class, IEntity<TId>
{
    protected readonly ILogger _logger;
    protected readonly IDescribes _describes;
    protected readonly IMapper _mapper;
    private readonly IValidator<TDTORequest> _validator;

    protected FunctionBase(ILogger logger, IDescribes describes, IMapper mapper, IValidator<TDTORequest> validator)
    {
        _logger = logger;
        _describes = describes;
        _mapper = mapper;
        _validator = validator;
    }







    /// <summary>
    /// 
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">Não foi possível achar a query na requisição</exception>
    /// <exception cref="FormatException">Não foi possível decodificar a serialização da query</exception>
    /// <exception cref="JsonException">Erro na serialização</exception>

    protected Result<EntityQuery> GetEntityQuery(HttpRequestData req){
        var encodedQuery = req.Query["query"];
        if(encodedQuery == null)
            return Result<EntityQuery>.Failure(new Error(((int)HttpStatusCode.BadRequest), _describes.QueryMissing()));
        
        

        var query = HttpUtility.UrlDecode(encodedQuery);
        if(query == null){
            var eventId = new EventId(id: Values.Events.QueryDecode);
            var message = _describes.UnknownJsonFormat();
            _logger.LogError(eventId, message);
            return Result<EntityQuery>.Failure(new Error(Values.Events.QueryDecode, message));
        }

        try
        {
            var entityQuery = JsonConvert.DeserializeObject<EntityQuery>(query);
            return Result<EntityQuery> .Success(entityQuery);
        }
        catch (System.Exception e)
        {
            var eventId = new EventId(Values.Events.QueryDecode);
            var message = _describes.UnknownJsonFormat();
            _logger.LogError(eventId, message);
            return Result<EntityQuery>.Failure(new Error(Values.Events.QueryConvert, message));
        }

        
    }
    protected async ValueTask<Result<TDTO>> GetJsonFromBody<TDTO>(HttpRequestData req)
    {
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        try
        {
            var dto = JsonConvert.DeserializeObject<TDTO>(body);
            if(dto == null)
                return Result<TDTO>.Failure(new Error(((int)HttpStatusCode.BadRequest), _describes.UnknownJsonFormat()));
            return Result<TDTO>.Success(dto!);
            
        }
        catch (JsonException)
        {
            return Result<TDTO>.Failure(new Error(((int)HttpStatusCode.BadRequest), _describes.UnknownJsonFormat()));
            throw;
        }
    }
    protected async ValueTask<IActionResult> ActionGetByQuery(HttpRequestData req, Func<IEntityQuery, IQueryable> queryFunc){
        var entityQueryProcessed = GetEntityQuery(req);
        if (entityQueryProcessed.IsFailure)
        {
            return new JsonResult(entityQueryProcessed.Error)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
        var final = await queryFunc.Invoke(entityQueryProcessed.Model!).ToDynamicArrayAsync();
        return new JsonResult(final)
        {
            StatusCode = StatusCodes.Status200OK
        };
    }
    protected async ValueTask<IActionResult> ActionGetByIndex<TIndex>(TIndex indexValue, Func<TIndex, ValueTask<TEntity?>> getFunction){
        var entity = await getFunction.Invoke(indexValue);
        if (entity == null)
            return new NotFoundObjectResult(indexValue);
        var response = _mapper.Map<TEntity, TDTORequest>(entity);
        return new JsonResult(response)
        {
            StatusCode = StatusCodes.Status200OK
        };
    }
    protected ValueTask<IActionResult> ActionGetById(TId id, Func<TId, ValueTask<TEntity?>> getFunction)
        => ActionGetByIndex(id, getFunction);
    protected async ValueTask<IActionResult>ActionCreateEntity(HttpRequestData httpRequest, Func<TEntity, ValueTask> addFunction, string[]? validationRulesets = null){
        validationRulesets ??= new string[] { "default" };
        
        var dto = await GetJsonFromBody<TDTORequest>(httpRequest);

        if (dto.IsFailure)
            return new JsonResult(dto.Error)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };


        var results = await _validator.ValidateAsync(dto.Model, opt => opt.IncludeRuleSets("default", TagValidator.RuleSetAdd));
        var modelState = ModelState.FromValidationResult(results);
        if (!modelState.IsValid)
            return new JsonResult(modelState.GroupByProperty())
            {
                StatusCode = StatusCodes.Status422UnprocessableEntity
            };

        var entity = _mapper.Map<TDTORequest, TEntity>(dto.Model);
        await addFunction.Invoke(entity);

        var responseDTO = _mapper.Map<TEntity, TDTOResponse>(entity);

        return new JsonResult(responseDTO)
        {
            StatusCode = StatusCodes.Status201Created
        };
    }

    protected async ValueTask<IActionResult> ActionUpdateByIndex<TIndex>(HttpRequestData req, TIndex indexValue, Func<TEntity, TIndex, ValueTask> updateFunction, Func<TIndex, ValueTask<bool>> existsFunction, string[]? validationRuleSets = null){
         var dto = await GetJsonFromBody<TDTORequest>(req);

        if (dto.IsFailure)
            return new JsonResult(_describes.BadBodyRequest())
            {
                StatusCode = StatusCodes.Status400BadRequest
            };


        if (! await existsFunction.Invoke(indexValue))
            return new JsonResult(_describes.KeyNotFound(indexValue!))
            {
                StatusCode = StatusCodes.Status404NotFound
            };

        var results = await _validator.ValidateAsync(dto.Model, opt => opt.IncludeRuleSets(validationRuleSets));

        var modelState = ModelState.FromValidationResult(results);
        if (!modelState.IsValid)
            return new JsonResult(modelState.AsPropertyAndMessages())
            {
                StatusCode = StatusCodes.Status422UnprocessableEntity
            };

        var entity = _mapper.Map<TDTORequest, TEntity>(dto.Model);
        await updateFunction.Invoke(entity, indexValue);


        var responseDTO = _mapper.Map<TEntity, TDTOResponse>(entity);

        return new JsonResult(responseDTO)
        {
            StatusCode = StatusCodes.Status200OK
        };
    }

    protected ValueTask<IActionResult> ActionUpdate(HttpRequestData req, TId id, Func<TEntity, TId, ValueTask> updateFunction, Func<TId, ValueTask<bool>> existsFunction, string[]? validationRuleSets = null)
        => ActionUpdateByIndex(req, id, updateFunction, existsFunction, validationRuleSets);

    protected async ValueTask<IActionResult> ActionDeleteByIndex<TIndex>(TIndex indexValue, Func<TIndex, ValueTask> deleteFunction, Func<TIndex, ValueTask<bool>> existsFunction){
        if(! await existsFunction.Invoke(indexValue))
            return new JsonResult(_describes.KeyNotFound(indexValue!))
            {
                StatusCode = StatusCodes.Status404NotFound
            };
            
        await deleteFunction.Invoke(indexValue);
        return new JsonResult(null)
        {
            StatusCode = StatusCodes.Status200OK
        };
    
    }

    protected ValueTask<IActionResult> ActionDelete(TId id , Func<TId, ValueTask> deleteFunction, Func<TId, ValueTask<bool>> existsFunction)
        => ActionDeleteByIndex(id, deleteFunction, existsFunction);
}
