using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Net;
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
    private readonly IValidator<TagRequestDTO> _validator;

    public TagFunctions(IMapper mapper, ITagRepository repository, IDescribes describes, IValidator<TagRequestDTO> validator)
    {
        _mapper = mapper;
        _repository = repository;
        _describes = describes;
        _validator = validator;
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

    [Function(Values.Api.TagAdd)]
    [OpenApiOperation(Values.Api.TagAdd, Description = "Adiciona uma tag.")]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MimeMapping.KnownMimeTypes.Json, typeof(TagResponseDTO[]))]
    public async Task<IActionResult> Add([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req){

        var dto = await req.ReadFromJsonAsync<TagRequestDTO>();
        if(dto == null)
            return new BadRequestObjectResult(_describes.BadBodyRequest());

        var results = await _validator.ValidateAsync(dto);
        var modelState = ModelState.FromValidationResult(results);
        if(!modelState.IsValid)
            return new UnprocessableEntityObjectResult(modelState.GroupByProperty());

        var entity = _mapper.Map<TagRequestDTO, Tag>(dto);
        await _repository.Add(entity);
    
        var responseDTO = _mapper.Map<Tag, TagResponseDTO>(entity);
        return new CreatedAtActionResult(Values.Api.TagAdd, null, null, responseDTO);
    }
}
