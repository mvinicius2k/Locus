﻿using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using Api.Models;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Models;

namespace Api;

public class UserFunctions : FunctionBase<User, string, UserRequestDTO, UserResponseDTO>
{
    private readonly IUserRepository _users;

    public UserFunctions(ILogger<UserFunctions> logger, IDescribes describes, IMapper mapper, IUserRepository userRepository) : base(logger, describes, mapper, null)
    {
        _users = userRepository;
    }

    [Function("Register-User")]
    [OpenApiOperation(Values.Api.UserRegister, Description = "Cria um post")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, MimeMapping.KnownMimeTypes.Json, typeof(UserResponseDTO))]
    [OpenApiResponseWithBody(HttpStatusCode.UnprocessableEntity, MimeMapping.KnownMimeTypes.Json, typeof(Dictionary<string, List<string>>))]
    public async Task<IActionResult> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Values.Api.UserRegister)] HttpRequestData req)
    {
        var jsonResult = await GetJsonFromBody<UserRequestDTO>(req);

        var user = _mapper.Map<UserRequestDTO, User>(jsonResult.Model);

        var register = await _users.Register(user, jsonResult.Model.RawPassword);
        if (register.Succeeded)
        {

            var response = _mapper.Map<User, UserResponseDTO>(user);
            return new JsonResult(response)
            {
                StatusCode = StatusCodes.Status201Created
            };
        }

        return new BadRequestObjectResult(register.Errors);
    }

    [Function("SignIn-User")]
    [OpenApiOperation(Values.Api.UserSignIn, Description = "Cria um post")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, MimeMapping.KnownMimeTypes.Json, typeof(JwtTokenResponseDTO))]
    [OpenApiResponseWithBody(HttpStatusCode.UnprocessableEntity, MimeMapping.KnownMimeTypes.Json, typeof(Dictionary<string, List<string>>))]
    public async Task<IActionResult> SignIn([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Values.Api.UserSignIn)] HttpRequestData req)
    {
        var jsonResult = await GetJsonFromBody<UserLoginDTO>(req);
        if (jsonResult.IsFailure)
        {
            return new BadRequestObjectResult(jsonResult.Error);
        }

        var model = jsonResult.Model;

        var verification = await _users.SignIn(model.Email, model.Password, model.RememberMe, req.FunctionContext.GetHttpContext());

        if (verification.IsFailure)
        {
            return new UnauthorizedObjectResult(verification.Error);
        }

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(verification.Model);
        return new JsonResult(new JwtTokenResponseDTO(jwtToken, verification.Model.ValidTo))
        {
            StatusCode = StatusCodes.Status200OK
        };


    }

    [Function("IsAuthenticated-User")]
    [AuthRequired]
    [OpenApiOperation(Values.Api.UserIsAuthenticated, Description = "Verifica se o usuário está logado")]
    public async Task<IActionResult> IsAuthenticated([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Values.Api.UserIsAuthenticated)] HttpRequestData req)
    {
        var user = req.FunctionContext.GetHttpContext().User;
        var identifier = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if(identifier == null)
            return new UnauthorizedObjectResult("Nome nao encontrado");
        return new OkObjectResult("POde ir");
    }
    
    [Function("Foo-User")]
    [OpenApiOperation(Values.Api.UserFoo, Description = "Faz nada")]
    public async Task<IActionResult> Foo([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Values.Api.UserFoo)] HttpRequestData req)
        => new OkObjectResult("Ok");


}


