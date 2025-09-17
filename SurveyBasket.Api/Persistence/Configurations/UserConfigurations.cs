namespace SurveyBasket.Api.Persistence.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder
            .OwnsMany(x=>x.RefreshTokens)
            .ToTable("ApplicationUserRefreshTokens")
            .WithOwner()
            .HasForeignKey("UserId");

        builder.Property(u => u.FirstName)
            .HasMaxLength(100);
              
        builder.Property(u => u.LastName)
            .HasMaxLength(100);


    }
}
