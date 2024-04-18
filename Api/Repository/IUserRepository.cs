using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Database;
using Api.Helpers;
using Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared;
using Shared.Models;

namespace Api;

public interface IUserRepository
{
    public ValueTask<User?> GetById(string id);
    public ValueTask<User?> GetByEmail(string email);
    public ValueTask<IdentityResult> Register(User user, string rawPassword);
    public ValueTask<Result<JwtSecurityToken>> SignIn(string email, string password, bool rememberMe, HttpContext httpContext);
    public ValueTask SignOut();
    public ValueTask Delete(string id);
    public ValueTask<IdentityResult> Update(User user, string id);
    public ValueTask UpdateInfos(IUserInfos userInfos);



}

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly Context _context;
    private readonly IConfiguration _configuration;
    private readonly IDescribes _describes;

    public UserRepository(UserManager<User> userManager, SignInManager<User> signInManager, Context context, IConfiguration configuration, IDescribes describes)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _configuration = configuration;
        _describes = describes;
    }

    public ValueTask Delete(string id)
    {
        throw new NotImplementedException();
    }

    public ValueTask<User?> GetByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public ValueTask<User?> GetById(string id)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<IdentityResult> Register(User user, string rawPassword)
        => await _userManager.CreateAsync(user, rawPassword);

    public async ValueTask<Result<JwtSecurityToken>> SignIn(string email, string password, bool rememberMe, HttpContext httpContext)
    {

        var user = _context.Users.FirstOrDefault(u => u.NormalizedEmail == email.ToUpper());
        if (user == null)
            return Result<JwtSecurityToken>.Failure(new Error(StatusCodes.Status404NotFound, _describes.KeyNotFound(email)));

        var userName = email;
        _signInManager.Context = httpContext;
        var passVerification = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!passVerification.Succeeded)
            return Result<JwtSecurityToken>.Failure(new Error(StatusCodes.Status403Forbidden, passVerification.ToString()));

        var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var userRole in userRoles)
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));

        var token = GetToken(authClaims);
        return Result<JwtSecurityToken>.Success(token);
        
    }


    public async ValueTask SignOut()
        => await _signInManager.SignOutAsync();

    public async ValueTask<IdentityResult> Update(User user, string id)
    {



        user.SetId(id);
        return await _userManager.UpdateAsync(user);
    }

    public ValueTask UpdateInfos(IUserInfos userInfos)
    {
        throw new NotImplementedException();
    }

    private JwtSecurityToken GetToken(IEnumerable<Claim> authClaims)
    {
        var children = Environment.GetEnvironmentVariables();
        

        var jwtSecret = Environment.GetEnvironmentVariable(ApiValues.GoogleOAuthSecretEnvKey);
        var validAudience = Environment.GetEnvironmentVariable(ApiValues.JWTValidAudienceEnvKey);
        var validIssuer = Environment.GetEnvironmentVariable(ApiValues.JWTValidIssuerEnvKey);


        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)) ?? throw new KeyNotFoundException("Variável de ambiente nao encontrada");

        var token = new JwtSecurityToken(
            issuer: validIssuer,
            audience: validAudience,
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

        return token;
    }
}

