namespace SurveyBasket.Api.Extentions;

public static class UserExtentions
{
    public static string? GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
