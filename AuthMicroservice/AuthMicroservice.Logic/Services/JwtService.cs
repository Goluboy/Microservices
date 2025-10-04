using AuthMicroservice.DAL.Models;
using AuthMicroservice.Logic.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthMicroservice.Logic.Services;

public class JwtService(UserManager<UserDal> userManager) : IJwtService
{
    public const string ISSUER = "MyAuthServer";
    public const string AUDIENCE = "MyAuthClient";

    private const string KEY = "mysercretkeyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy345354235t425t342t55tt5ttt23t55553t53io5353i53o2235oimnk";

    public static SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.UTF8.GetBytes(KEY));

    public async Task<string> GenerateAccessTokenAsync(UserDal user)
    {
        var roles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        if (!string.IsNullOrEmpty(user.FirstName))
            claims.Add(new Claim("first_name", user.FirstName));

        if (!string.IsNullOrEmpty(user.LastName))
            claims.Add(new Claim("last_name", user.LastName));

        var token = new JwtSecurityToken(
            issuer: ISSUER,
            audience: AUDIENCE,
            claims: claims,
            expires: DateTime.Now.AddMinutes(1),
            signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public (bool, string) ValidateTokenAsync(string accessToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = ISSUER,
            ValidAudience = AUDIENCE,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY)),

        };

        try
        {
            var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out var validatedToken);
            return (true, principal.Claims.ToString());
        }
        catch (Exception e)
        {
            return (false, e.ToString());
        }

        
    }
}