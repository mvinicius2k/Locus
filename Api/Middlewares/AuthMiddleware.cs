using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Api.Helpers;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared;
using Shared.Helpers;

namespace Api;

public class AuthMiddleware : IFunctionsWorkerMiddleware
{
    private readonly ILogger<AuthMiddleware> _logger;
    private readonly IDescribes _describes;

    public AuthMiddleware(ILogger<AuthMiddleware> logger, IDescribes describes)
    {
        _logger = logger;
        _describes = describes;
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        _logger.LogInformation("Iniciando rotina de auth");

        var request = await context.GetHttpRequestDataAsync();
        if (!request.Headers.TryGetValues(Values.Api.HeaderAuthorizationKey, out var keys))
        {

            await ApplyUnauthrizedResponse(context, request, _describes.LoginRequired());
            return;
        }
        var jwtToken = keys.First().Split(" ").Last();


        try
        {
            var jwtData = await GoogleJsonWebSignature.ValidateAsync(jwtToken);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, jwtData.Email),
                new Claim(ClaimTypes.NameIdentifier, jwtData.Email),
            };

            var identity = new ClaimsIdentity(claims, "GoogleAuth");
            var principal = new ClaimsPrincipal(identity);
            context.GetHttpContext().User = principal;

        }
        catch (InvalidJwtException ex)
        {
            _logger.LogWarning("Token inválido: " + ex.Message);
            await ApplyUnauthrizedResponse(context, request, _describes.LoginRequired());
            return;
        }



        await next(context);




    }
    private async ValueTask ApplyUnauthrizedResponse(FunctionContext context, HttpRequestData? request, string message = "")
    {
        var redirect = Environment.GetEnvironmentVariable(ApiValues.GoogleOAuthCallbackEnvKey);
        var clientId = Environment.GetEnvironmentVariable(ApiValues.GoogleOAuthClientIdEnvKey);
        var googleOuthModel = new GoogleOAuthRedirectData(redirect, clientId);

        if (redirect == null || clientId == null)
            _logger.LogError($"Variável de redirecionamento para login do google ou de id não definida");
        var responseDTO = new AuthFailedDTO(googleOuthModel.ToUri().AbsoluteUri, message);
        var response = request.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
        response.Headers.Add("Content-Type", "application/json");

        //await response.WriteAsJsonAsync(responseDTO); dont work
        var serialized = JsonConvert.SerializeObject(responseDTO);
        var ms = new MemoryStream(Encoding.UTF8.GetBytes(serialized));
        await response.Body.WriteAsync(ms.ToArray());

        context.GetInvocationResult().Value = response;
    }




}

public record AuthFailedDTO(string LoginUrl, string Message);

public record GoogleOAuthRedirectData(string RedirectUri, string ClientId, string Scope = GoogleOAuthRedirectData.ScopeUserInfoEmail, string AccessType = "offline", bool IncludeGrantedScopes = true, string ResponseType = "code")
{
    public const string ScopeUserInfoEmail = "https://www.googleapis.com/auth/userinfo.email";
    public Uri ToUri()
    {
        var query = new NameValueCollection(){
            {"scope",Scope},
            {"access_type",AccessType},
            {"include_granted_scopes", IncludeGrantedScopes.ToString().ToLower()},
            {"response_type",ResponseType},
            {"redirect_uri",RedirectUri},
            {"client_id",ClientId},
        };

        var fullUrl = Values.Api.GoogleLoginUrl + query.ToQueryWithPrefix();
        return new Uri(fullUrl);
    }
};