namespace SurveyBasket.Api.Persistence.Configurations;

public class AnswerConfigurations :IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
      
        builder
            .HasIndex(a => new { a.QuestionId, a.Content }).IsUnique();

        builder
            .Property(a => a.Content)
            .IsRequired()
            .HasMaxLength(1000);

    }
}

