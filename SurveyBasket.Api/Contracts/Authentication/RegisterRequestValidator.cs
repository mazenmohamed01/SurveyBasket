using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Contracts.Authentication;

public class RegisterRequestValidator:AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Matches(RegexPatterns.Password)
            .WithMessage("Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one number, and one special character.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .Matches("^[a-zA-Z]+$").WithMessage("First name must contain only letters.")
            .Matches(@"^[A-Z][a-z]*$").WithMessage("First name must start with a capital letter.")
            .Length(3,100).WithMessage("First name must be between 3 and 100 characters long.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .Length(3,100).WithMessage("Last name must be between 3 and 100 characters long.")
            .Matches("^[a-zA-Z]+$").WithMessage("Last name must contain only letters.")
            .Matches(@"^[A-Z][a-z]*$").WithMessage("Last name must start with a capital letter.");
    }
}
