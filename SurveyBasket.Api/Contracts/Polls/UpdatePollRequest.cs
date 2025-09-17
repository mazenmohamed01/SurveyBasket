namespace SurveyBasket.Api.Contracts.Polls;

public record UpdatePollRequest
(
    string Title,
    string Summary,
    DateOnly StartsAt,
    DateOnly EndsAt
);
