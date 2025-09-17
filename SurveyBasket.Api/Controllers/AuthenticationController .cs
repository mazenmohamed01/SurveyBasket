

namespace SurveyBasket.Api.Controllers;
[Route("[controller]")]
[ApiController]
public class AuthenticationController(IAuthenticationService authenticationService,ILogger<AuthenticationController> logger) : ControllerBase
{
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly ILogger<AuthenticationController> _logger = logger;

    [HttpPost("")]                                                
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Loging With email: {email} and eassword: {password} ", request.Email,request.Password);

        var authResult = await _authenticationService.GetTokenAsync(request.Email, request.Password, cancellationToken);

        return authResult.IsSuccess
            ? Ok(authResult.Value)
            : authResult.ToProblem();

    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var authResult = await _authenticationService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return authResult.IsSuccess 
            ? Ok(authResult.Value) 
            : authResult.ToProblem();
    }

    [HttpPost("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authenticationService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return result.IsSuccess 
            ? Ok() 
            : result.ToProblem();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var registrationResult = await _authenticationService.RegisterAsync(request, cancellationToken);
        
        return registrationResult.IsSuccess 
            ? Ok(registrationResult.Value) 
            : registrationResult.ToProblem();
    }
}
