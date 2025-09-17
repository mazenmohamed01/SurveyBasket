namespace SurveyBasket.Api.Contracts.Authentication;

public class RefreshTokenRequestValidatore: AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidatore()
    {
        RuleFor(x => x.Token)
            .NotEmpty();
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}

