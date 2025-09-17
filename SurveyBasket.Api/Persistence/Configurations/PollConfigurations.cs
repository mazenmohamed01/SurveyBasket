namespace SurveyBasket.Api.Persistence.Configurations;

public class PollConfigurations : IEntityTypeConfiguration<Poll>
{
    public void Configure(EntityTypeBuilder<Poll> builder)
    {
        builder.HasIndex(p => p.Title).IsUnique();
        builder.Property(p=> p.Title)
            .HasMaxLength(100);

        builder.Property(p => p.Summary)
            .HasMaxLength(1500);
        // Additional configurations can be added here if needed
        // For example, configuring the date properties or IsPublished flag
        //builder.Property(p => p.IsPublished)
        //    .IsRequired()
        //    .HasDefaultValue(false);
        
        //builder.Property(p => p.StartsAt)
        //    .IsRequired();
        
        //builder.Property(p => p.EndsAt)
        //    .IsRequired();
    }
}
