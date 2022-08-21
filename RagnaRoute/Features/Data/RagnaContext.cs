using Microsoft.EntityFrameworkCore;
using RagnaRoute.Entities;

namespace RagnaRoute.Data;
public class RagnaContext : DbContext
{
    public DbSet<QuestCompletion> BossQuestCompletions { get; set; } = null!;

    public RagnaContext(DbContextOptions<RagnaContext> options) : base(options)
    {
    }
}
