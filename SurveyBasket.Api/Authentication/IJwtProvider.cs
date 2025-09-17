using Microsoft.Extensions.Options;

namespace SurveyBasket.Api.Authentication;

public interface IJwtProvider
{
    (string Token,int Expire) GenerateToken(ApplicationUser user);
    string? ValidateToken(string token);
}
