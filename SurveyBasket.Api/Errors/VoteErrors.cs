namespace SurveyBasket.Api.Errors;

public static class VoteErrors
{
    public static readonly Error UserAlreadyVoted =
        new("UserAlreadyVoted", "You have already voted in this poll.", StatusCodes.Status400BadRequest);
    public static readonly Error InvalidQuestions =
        new("InvalidQuestions", "The provided questions are invalid or do not match the poll's questions.", StatusCodes.Status400BadRequest);
}
