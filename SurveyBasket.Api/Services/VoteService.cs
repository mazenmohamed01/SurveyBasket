
using SurveyBasket.Api.Contracts.Votes;

namespace SurveyBasket.Api.Services;

public class VoteService(AppDbContext context) : IVoteService
{
    private readonly AppDbContext _context = context;

    public async Task<Result> AddAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken = default)
    {
        var hasVote = await _context.Votes
            .AnyAsync(v => v.PollId == pollId && v.UserId == userId, cancellationToken);

        if (hasVote)
            return Result.Failure(VoteErrors.UserAlreadyVoted);

        var pollIsExist = await _context.Polls
            .AnyAsync(p => p.Id == pollId && p.IsPublished && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

        if (!pollIsExist)
            return Result.Failure(PollErrors.PollNotFound);

        var availableQuestions = await _context.Questions
            .Where(q => q.PollId == pollId && q.IsActive)
            .Select(q => q.Id)
            .ToListAsync(cancellationToken);

        if(!request.Answers.Select(a => a.QuestionId).SequenceEqual(availableQuestions))
            return Result.Failure(VoteErrors.InvalidQuestions);

        var vote = new Vote
        {
            PollId = pollId,
            UserId = userId,
            VoteAnswers = request.Answers.Adapt<IEnumerable<VoteAnswer>>().ToList()
        };

        await _context.AddAsync(vote, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
