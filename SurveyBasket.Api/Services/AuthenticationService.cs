
using SurveyBasket.Api.Authentication;
using SurveyBasket.Api.Errors;
using System.Security.Cryptography;

namespace SurveyBasket.Api.Services;

public class AuthenticationService(UserManager<ApplicationUser> userManager,IJwtProvider jwtProvider) : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

    private readonly int _refreshTokenExpirationDays = 14; 

    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        //Check User?
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials); // User not found

        //Check Password
        var IsValidPassword = await _userManager.CheckPasswordAsync(user, password);

        if (!IsValidPassword)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials); // Invalid password

        //Generate Token?
        var (token, ExpiresIn) = _jwtProvider.GenerateToken(user);

        //Generate Refresh Token
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);

        user.RefreshTokens.Add(new ApplicationUserRefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        //Save User with Refresh Token
        await _userManager.UpdateAsync(user);

        var response = new AuthResponse
        (
            user.Id,
            user.Email,
            user.FirstName ?? string.Empty,
            user.LastName ?? string.Empty,
            token,
            ExpiresIn,
            refreshToken,
            refreshTokenExpiration
        );
        return Result.Success(response);
    }

    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId= _jwtProvider.ValidateToken(token);
        if (userId is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken && rt.IsActive);
        if (userRefreshToken is null) 
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken); // Invalid or expired refresh token

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        //Generate Token
        var (newtoken, ExpiresIn) = _jwtProvider.GenerateToken(user);

        //Generate Refresh Token
        var newrefreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);

        user.RefreshTokens.Add(new ApplicationUserRefreshToken
        {
            Token = newrefreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        //Save User with Refresh Token
        await _userManager.UpdateAsync(user);

        var response = new AuthResponse
        (
            user.Id,
            user.Email,
            user.FirstName ?? string.Empty,
            user.LastName ?? string.Empty,
            newtoken,
            ExpiresIn,
            newrefreshToken,
            refreshTokenExpiration
        );

        return Result.Success(response);
    }

    public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);
        if (userId is null)
            return Result.Failure(UserErrors.InvalidJwtToken); // Invalid JWT token

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure(UserErrors.InvalidJwtToken); // User not found

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken && rt.IsActive);
        if (userRefreshToken is null)
            return Result.Failure(UserErrors.InvalidRefreshToken); // Invalid or expired refresh token

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        //Save User with Revoked Refresh Token
        await _userManager.UpdateAsync(user);
        return Result.Success();
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        //Check if User already exists
        var existingUser = await _userManager.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
        if (existingUser)
            return Result.Failure<AuthResponse>(UserErrors.DuplicatedEmail); // User already exists

        //Create new User
        var user = request.Adapt<ApplicationUser>();

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            //Generate Token
            var (token, ExpiresIn) = _jwtProvider.GenerateToken(user);
            //Generate Refresh Token
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);
            
            user.RefreshTokens.Add(new ApplicationUserRefreshToken
            {
                Token = refreshToken,
                ExpiresOn = refreshTokenExpiration
            });
            //Save User with Refresh Token
            await _userManager.UpdateAsync(user);
            var response = new AuthResponse
            (
                user.Id,
                user.Email,
                user.FirstName ?? string.Empty,
                user.LastName ?? string.Empty,
                token,
                ExpiresIn,
                refreshToken,
                refreshTokenExpiration
            );
            
            return Result.Success(response);

        }

        var error = result.Errors.First();

        return Result.Failure<AuthResponse>(new Error(error.Code, error.Description,400)); // Registration failed
    }
    private static string GenerateRefreshToken()
    {
        // Generate a secure random token (you can use a library or your own method)
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)); 
    }


}
