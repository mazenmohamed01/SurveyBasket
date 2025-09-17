namespace SurveyBasket.Api.Errors;

public class QuestionErrors
{
    public static readonly Error DuplicatedQuestionContent = new(
        "Question.DuplicatedContent",
        "Another Question With The Same content is already exists for this poll.",
        StatusCodes.Status409Conflict);

    public static readonly Error QuestionNotFound = new(
        "Question.NotFound",
        "Question not found.",
        StatusCodes.Status404NotFound);

    public static readonly Error InvalidQuestionType = new(
        "Question.InvalidType",
        "Invalid question type provided.",
        StatusCodes.Status400BadRequest);
}
