using System.Linq.Dynamic.Core;
using System.Net;
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

public abstract class FunctionBase
{
    protected readonly ILogger _logger;
    protected readonly IDescribes _describes;

    protected FunctionBase(ILogger logger, IDescribes describes)
    {
        _logger = logger;
        _describes = describes;
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
}
