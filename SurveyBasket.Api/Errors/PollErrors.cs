namespace SurveyBasket.Api.Errors;

public class PollErrors
{
    public static readonly Error PollNotFound =
        new("Poll.NotFound", "No Poll Was found With The Given ID.", StatusCodes.Status404NotFound);

    public static readonly Error DuplicatedPollTitle =
        new("Poll.DuplicatedTitle", "A Poll with the same title already exists.", StatusCodes.Status409Conflict);
}
