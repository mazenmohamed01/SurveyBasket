using SurveyBasket.Api.Contracts.Answers;
using SurveyBasket.Api.Contracts.Questions;

namespace SurveyBasket.Api.Services;

public class QuestionService(AppDbContext context) : IQuestionService
{
    private readonly AppDbContext _context = context;


    public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var IsExistPoll = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
        if (!IsExistPoll)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound); // Poll not found

        var questions = await _context.Questions
            .Where(q => q.PollId == pollId)
            .Include(q => q.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
            
        return Result.Success<IEnumerable<QuestionResponse>>(questions);
    }

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, string userId, CancellationToken cancellationToken = default)
    {
        var hasVoted = await _context.Votes.AnyAsync(v => v.PollId == pollId && v.UserId == userId, cancellationToken);
        if (hasVoted)
            return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.UserAlreadyVoted); // User has already voted

        var pollIsExist = await _context.Polls
            .AnyAsync(p => p.Id == pollId && p.IsPublished && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);
       
        if (!pollIsExist)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound); // Poll not found

        var questions = await _context.Questions.Where(q => q.PollId==pollId && q.IsActive)
            .Include(q => q.Answers)
            .Select(q => new QuestionResponse
            (
                q.Id,
                q.Content,
                q.Answers.Where(a => a.IsActive).Select(a => new AnswerResponse(a.Id, a.Content))
            ))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<QuestionResponse>>(questions);

    }

    public async Task<Result<QuestionResponse>> GetAsync(int pollId, int questionId, CancellationToken cancellationToken = default)
    {
        var question = await _context.Questions
            .Where(q => q.PollId == pollId && q.Id == questionId)
            .Include(q => q.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (question is null)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound); // Question not found

        return Result.Success(question);
    }

    public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var IsExistPoll = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
        if (!IsExistPoll)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound); // Poll not found

        var IsExistingQuestion = await _context.Questions.AnyAsync(q => q.PollId == pollId && q.Content == request.Content, cancellationToken);
        
        if (IsExistingQuestion)
            return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedQuestionContent); // Question content already exists

        var newQuestion = request.Adapt<Question>();
        newQuestion.PollId = pollId; // Set the PollId for the new question

        await _context.AddAsync(newQuestion, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(newQuestion.Adapt<QuestionResponse>());
    }


    public async Task<Result> UpdateAsync(int pollId, int questionId, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var IsExistingQuestion = await _context.Questions
            .AnyAsync(q => q.PollId == pollId && q.Id != questionId && q.Content == request.Content, cancellationToken);
        
        if (IsExistingQuestion)
            return Result.Failure(QuestionErrors.DuplicatedQuestionContent); // Question content already exists

        var question = await _context.Questions
            .Include(a => a.Answers)
            .SingleOrDefaultAsync(q => q.PollId == pollId && q.Id == questionId, cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound); // Question not found

        question.Content = request.Content; // Update the content of the question
        
        //Current answers
        var currentAnswers = question.Answers.Select(q=> q.Content).ToList();

        // New answers from request
        var newAnswers = request.Answers.Except(currentAnswers).ToList();

        newAnswers.ForEach(answer =>
        {
            question.Answers.Add(new Answer { Content = answer });
        });

        question.Answers.ToList().ForEach(answer =>
        {
            answer.IsActive = request.Answers.Contains(answer.Content); // Update the status of existing answers based on the request
        });
            
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }


    public async Task<Result> ToggelStatusAsync(int pollId, int questionId, CancellationToken cancellationToken = default)
    {
        var question = await _context.Questions
            .Where(q => q.PollId == pollId && q.Id == questionId)
            .SingleOrDefaultAsync(cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound); // Question not found

        question.IsActive = !question.IsActive; // Toggle the status

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

}
