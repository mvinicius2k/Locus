using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Net;
using Api.Database;
using Api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shared;
using Shared.Api;
using Shared.Models;

namespace Api;

public class TagFunctions
{
    
    private readonly IMapper _mapper;
    private readonly ITagRepository _repository;
    private readonly IDescribes _describes;
    

    public TagFunctions(IMapper mapper, ITagRepository repository, IDescribes describes)
    {
        _mapper = mapper;
        _repository = repository;
        _describes = describes;
    }

    [Function(Values.Api.TagGet)]
    [OpenApiOperation(Values.Api.TagGet, Description = "Obtém uma consulta de tags.")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MimeMapping.KnownMimeTypes.Json, typeof(TagResponseDTO[]))]
    public IActionResult Get([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        var query = req.Query[Values.Api.TagGetQuery];
        if(query == null)
            return new BadRequestObjectResult(_describes.QueryMissing());
            
        var entityQuery = JsonConvert.DeserializeObject<IEntityQuery>(query);

        if(entityQuery == null)
            return new BadRequestObjectResult(_describes.BadObject(Values.Api.TagGetQuery));

        var final = _repository.GetWithQuery(entityQuery);


        return new OkObjectResult(final);
    }
}
