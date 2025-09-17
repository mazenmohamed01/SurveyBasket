namespace SurveyBasket.Api.Contracts.Polls;

public class PollRequestValidator :AbstractValidator<CreatePollRequest>
{
    public PollRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Please add a {PropertyName}.")
            .Length(3,100)
            .WithMessage("Title must be between {MinLength} and {MaxLength} characters, You entered[{TotalLength}].");
        RuleFor(x => x.Summary)
            .NotEmpty()
            .WithMessage("Summary is required.")
            .Length(3,1500)
            .WithMessage("Description must be between {MinLength} and {MaxLength} characters, You entered[{TotalLength}]!");
        RuleFor(x => x.StartsAt)
            .NotEmpty()
            .WithMessage("Start date is required.")
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));
        RuleFor(x => x.EndsAt)
            .NotEmpty()
            .WithMessage("End date is required.");

        RuleFor(x=>x)
            .Must(HasValidDateRange)
            .WithName(nameof(CreatePollRequest.EndsAt))
            .WithMessage("{PropertyName} must be greater than or equal to Start date.");

    }

    private bool HasValidDateRange(CreatePollRequest createPollRequest)
    {
        return createPollRequest.EndsAt >= createPollRequest.StartsAt;
    }
}

