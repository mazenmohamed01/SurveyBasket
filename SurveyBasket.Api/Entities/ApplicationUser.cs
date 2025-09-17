using Microsoft.AspNetCore.Identity;

namespace SurveyBasket.Api.Entities;

public sealed class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public List<ApplicationUserRefreshToken> RefreshTokens { get; set; } = [];

    //public DateOnly DateOfBirth { get; set; }
    //public string? ProfilePictureUrl { get; set; }
    //public bool IsActive { get; set; } = true;
    //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    //public DateTime? UpdatedAt { get; set; }

}
