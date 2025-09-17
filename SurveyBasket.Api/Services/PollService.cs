

namespace SurveyBasket.Api.Services;

public class PollService(AppDbContext context) : IPollService
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var polls = await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);
        return polls.Adapt<IEnumerable<PollResponse>>();
    }

    public async Task<IEnumerable<PollResponse>> GetCurrentAsync(CancellationToken cancellationToken = default)=>
        await _context.Polls
            .AsNoTracking()
            .Where(p => p.IsPublished && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow))
            .ProjectToType<PollResponse>()
            .ToListAsync(cancellationToken);


    public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default) 
    {
        var poll = await _context.Polls.FindAsync(id, cancellationToken);

        return poll is not null 
            ? Result.Success(poll.Adapt<PollResponse>())
            : Result.Failure<PollResponse>(PollErrors.PollNotFound); // Poll not found
    }
 


    public async Task<Result<PollResponse>> AddAsync(CreatePollRequest request, CancellationToken cancellationToken =default)
    {
        var IsExistingTitle = await _context.Polls.AnyAsync(p => p.Title == request.Title, cancellationToken);

        if (IsExistingTitle)
            return Result.Failure<PollResponse>(PollErrors.DuplicatedPollTitle); // Poll title already exists 

        var newPoll = request.Adapt<Poll>();

        await _context.AddAsync(newPoll, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        
        return Result.Success(newPoll.Adapt<PollResponse>());
    }

    public async Task<Result> UpdateAsync(int id, UpdatePollRequest request, CancellationToken cancellationToken = default)
    {
        var IsExistingTitle = await _context.Polls.AnyAsync(p => p.Title == request.Title && p.Id != id, cancellationToken);
        if (IsExistingTitle)
            return Result.Failure(PollErrors.DuplicatedPollTitle); // Poll title already exists

        var currentPoll = await _context.Polls.FindAsync(id, cancellationToken);

        if (currentPoll is null)
            return Result.Failure(PollErrors.PollNotFound); // Poll not found

        currentPoll.Title = request.Title;
        currentPoll.Summary = request.Summary;
        currentPoll.StartsAt = request.StartsAt;
        currentPoll.EndsAt = request.EndsAt;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var Poll = await _context.Polls.FindAsync(id, cancellationToken);

        if (Poll is null)
            return Result.Failure(PollErrors.PollNotFound); // Poll not found

        _context.Polls.Remove(Poll);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync(id, cancellationToken);

        if (poll is null)
            return Result.Failure(PollErrors.PollNotFound); // Poll not found

        poll.IsPublished = !poll.IsPublished;
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
