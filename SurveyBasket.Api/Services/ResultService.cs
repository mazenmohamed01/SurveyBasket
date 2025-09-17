

namespace SurveyBasket.Api.Services;

public class ResultService(AppDbContext context) : IResultService
{
    private readonly AppDbContext _context = context;

    public async Task<Result<PollVotesResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollVotes = await _context.Polls
            .Where(x => x.Id == pollId)
            .Select(x => new PollVotesResponse(
                x.Title,
                x.Votes.Select(v => new VoteResponse(
                    $"{v.User.FirstName} {v.User.LastName}",
                    v.SubmitedOn,
                    v.VoteAnswers.Select(a => new QuestionAnswerResponse(
                        a.Question.Content,
                        a.Answer.Content
                    ))
                ))
            ))
            .SingleOrDefaultAsync(cancellationToken);

        return pollVotes is null
            ? Result.Failure<PollVotesResponse>(PollErrors.PollNotFound)
            : Result.Success(pollVotes);
    }

    public async Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollIsExist = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken);

        if (!pollIsExist)
            return Result.Failure<IEnumerable<VotesPerDayResponse>>(PollErrors.PollNotFound);

        var votesPerDay = await _context.Votes
            .Where(x => x.PollId == pollId)
            .GroupBy(x => new { Date = DateOnly.FromDateTime(x.SubmitedOn) })
            .Select(g => new VotesPerDayResponse(
                g.Key.Date,
                g.Count()
            ))
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<VotesPerDayResponse>>(votesPerDay);
    }

    public async Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollIsExist= await _context.Polls.AnyAsync(x=>x.Id == pollId, cancellationToken);
        
        if(!pollIsExist)
            return Result.Failure<IEnumerable<VotesPerQuestionResponse>>(PollErrors.PollNotFound);

        var votesPerQuestion = await _context.VoteAnswers
            .Where(v => v.Vote.PollId == pollId)
            .Select(v=> new VotesPerQuestionResponse(
                v.Question.Content,
                v.Question.Votes
                .GroupBy(a=> new {AnswerId = a.Answer.Id,AnswerContent= a.Answer.Content})
                .Select(g=> new VotesPerAnswerResponse(
                    g.Key.AnswerContent,
                    g.Count()
                ))
            ))
            .ToListAsync (cancellationToken);

        return Result.Success<IEnumerable<VotesPerQuestionResponse>>(votesPerQuestion);
    }
}
