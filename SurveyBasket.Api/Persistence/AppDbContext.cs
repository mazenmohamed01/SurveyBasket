using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;

namespace SurveyBasket.Api.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options,IHttpContextAccessor httpContextAccessor ) : 
    IdentityDbContext<ApplicationUser>(options)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public DbSet<Poll> Polls { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<VoteAnswer> VoteAnswers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        var cascadeFKs = modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascadeFKs)
        fk.DeleteBehavior = DeleteBehavior.Restrict; // Change cascade delete to restrict delete

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<AuditableEntity>();

        foreach (var entry in entries)
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId()!;

            if (entry.State == EntityState.Added)
            {
                entry.Property(x=>x.CreatedById).CurrentValue = currentUserId; // Replace with actual user ID if available
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(x => x.UpdatedById).CurrentValue = currentUserId; // Replace with actual user ID if available
                entry.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;
            }
        }



        return base.SaveChangesAsync(cancellationToken);
    }
}
