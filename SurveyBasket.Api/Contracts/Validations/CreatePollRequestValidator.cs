
namespace SurveyBasket.Api.Contracts.Validations;

public class CreatePollRequestValidator :AbstractValidator<CreatePollRequest>
{
    public CreatePollRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Please add a {PropertyName}.")
            .Length(3,100)
            .WithMessage("Title must be between {MinLength} and {MaxLength} characters, You entered[{TotalLength}].");
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .Length(3,1000)
            .WithMessage("Description must be between {MinLength} and {MaxLength} characters, You entered[{TotalLength}]!");
    }
}

