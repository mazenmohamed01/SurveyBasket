namespace SurveyBasket.Api.Contracts.Questions;

public class QuestionRequestValidator:AbstractValidator<QuestionRequest>
{
    public QuestionRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content is required.")
            .Length(3,1000)
            .WithMessage("Content must be between 3 and 1000 characters long.");

        RuleFor(x => x.Answers)
            .NotNull();

        RuleFor(x => x.Answers)
            .Must(answers => answers.Count > 1)
            .WithMessage("At least two answers are required.")
            .When(x => x.Answers != null)
            .ForEach(answer => answer.NotEmpty()
            .WithMessage("Answer cannot be empty."));

        RuleFor(x => x.Answers)
            .Must(answers => answers.Distinct().Count() == answers.Count)
            .WithMessage("You cannot have duplicate answers For The Same Question.")
            .When(x => x.Answers != null);
    }
}
