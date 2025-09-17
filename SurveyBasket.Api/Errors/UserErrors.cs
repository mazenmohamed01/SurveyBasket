namespace SurveyBasket.Api.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials =
        new("User.InvalidCredentials", "Invalid email or password.",StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidJwtToken =
        new("User.InvalidJwtToken", "Invalid JWT token.", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidRefreshToken=
        new("User.InvalidRefreshToken", "Invalid refresh token.", StatusCodes.Status401Unauthorized);

    public static readonly Error DuplicatedEmail =
        new("User.DuplicatedEmail", "Email is already registered.", StatusCodes.Status409Conflict);
}
