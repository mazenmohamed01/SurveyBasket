
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyBasket.Api.Authentication;

public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions _options = options.Value;

    public (string Token, int Expire) GenerateToken(ApplicationUser user)
    {

        Claim[] claims = [
            new(JwtRegisteredClaimNames.Sub,user.Id),
            new(JwtRegisteredClaimNames.Email,user.Email!),
            new(JwtRegisteredClaimNames.GivenName,user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName,user.LastName),
            new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        ];

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)); // Replace with your actual secret key
        
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);


        var jwtSecurityToken = new JwtSecurityToken
        (
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes),
            signingCredentials: signingCredentials
        );

        return (Token: new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken), Expire: _options.ExpirationMinutes * 60);
    }

    public string? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = symmetricSecurityKey,
                ValidateIssuer = false,
                //ValidIssuer = _options.Issuer,
                ValidateAudience = false,
                //ValidAudience = _options.Audience,
                //ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // Optional: Set to zero to avoid clock skew issues
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            return jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
        }
        catch
        {
            return null;
        }
    }
}
