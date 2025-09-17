namespace SurveyBasket.Api.Entities;

[Owned]
public class ApplicationUserRefreshToken
{
    public string Token { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }= DateTime.UtcNow;
    public DateTime ExpiresOn { get; set; }
    public DateTime? RevokedOn { get; set; } = null;
    public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
    public bool IsActive => RevokedOn == null && !IsExpired;
}
