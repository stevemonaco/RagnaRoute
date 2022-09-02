using Microsoft.EntityFrameworkCore;
using RagnaRoute.Entities;

namespace RagnaRoute.Data;
public class RagnaContext : DbContext
{
    public DbSet<Family> Families { get; set; } = null!;
    public DbSet<Objective> Objectives { get; set; } = null!;
    public DbSet<Completion> Completions { get; set; } = null!;

    public RagnaContext(DbContextOptions<RagnaContext> options) : base(options)
    {
    }
}
