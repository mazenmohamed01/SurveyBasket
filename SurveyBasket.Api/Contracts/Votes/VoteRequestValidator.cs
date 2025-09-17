namespace SurveyBasket.Api.Contracts.Votes;

public class VoteRequestValidator:AbstractValidator<VoteRequest>
{
    public VoteRequestValidator()
    {        RuleFor(x => x.Answers)
            .NotEmpty()
            .WithMessage("Answers cannot be empty.");

        RuleForEach(x => x.Answers)
            .SetInheritanceValidator(v => v.Add(new VoteAnswerRequestValidator()));

    }
}
