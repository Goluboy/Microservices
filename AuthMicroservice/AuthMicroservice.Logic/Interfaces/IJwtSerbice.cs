using System.Security.Claims;
using AuthMicroservice.DAL.Models;

namespace AuthMicroservice.Logic.Interfaces;

public interface IJwtService
{
    Task<string> GenerateAccessTokenAsync(UserDal user);
    (bool, string) ValidateTokenAsync(string accessToken);
}