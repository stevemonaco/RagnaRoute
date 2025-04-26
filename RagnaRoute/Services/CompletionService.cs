using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using RagnaRoute.Data;
using RagnaRoute.Entities;

namespace RagnaRoute.Services;
public class CompletionService
{
	private readonly IDbContextFactory<RagnaContext> _dbFactory;

	public CompletionService(IDbContextFactory<RagnaContext> dbFactory)
	{
		_dbFactory = dbFactory;
	}

	public async Task AddCompletion(string familyName, string objectiveName, Instant completionTime)
	{
		using var context = _dbFactory.CreateDbContext();

		var family = await GetOrCreateFamily(context, familyName).ConfigureAwait(false);
		var objective = await GetOrCreateObjective(context, objectiveName, false, family).ConfigureAwait(false);
		objective.LastCompletion = completionTime;

		var completion = new Completion()
		{
			Family = family,
			Objective = objective,
			CompletionTime = completionTime
		};

		context.Completions.Add(completion);
		await context.SaveChangesAsync().ConfigureAwait(false);
	}

	public async Task<ICollection<CompletionDto>> GetCompletions(string familyName, string objectiveName)
	{
		using var context = _dbFactory.CreateDbContext();

		return await context.Completions.AsNoTracking()
			.Include(x => x.Family)
			.Include(x => x.Objective)
			.Where(x => x.Family.Name == familyName && x.Objective.Name == objectiveName)
			.Select(x => new CompletionDto(x.Family.Name, x.Objective.Name, x.CompletionTime))
			.ToListAsync()
			.ConfigureAwait(false);
	}

	public async Task<ICollection<string>> GetFamilyNames()
	{
        using var context = _dbFactory.CreateDbContext();

		return await context.Families.AsNoTracking()
			.Select(x => x.Name)
			.Distinct()
			.ToListAsync()
			.ConfigureAwait(false);
    }

	public async Task<ICollection<ObjectiveStateDto>> GetObjectivesForFamily(string familyName)
	{
        using var context = _dbFactory.CreateDbContext();

		var query = context.Objectives.AsNoTracking()
			.Include(x => x.Family)
			.Where(x => x.Family.Name == familyName);

        return await query
			.Select(x => new ObjectiveStateDto(familyName, x.Name, x.LastCompletion, x.IsHidden, x.IsFavorite))
			.ToListAsync()
			.ConfigureAwait(false);
    }

    public async Task<ICollection<ObjectiveStateDto>> GetCompletedObjectivesForFamily(string familyName)
    {
        using var context = _dbFactory.CreateDbContext();

        return await context.Objectives.AsNoTracking()
            .Include(x => x.Family)
            .Where(x => x.Family.Name == familyName)
			.Where(x => x.LastCompletion != null)
			.Select(x => new ObjectiveStateDto(familyName, x.Name, x.LastCompletion, x.IsHidden, x.IsFavorite))
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task UpsertObjectiveHiddenState(string familyName, string objectiveName, bool isHidden)
	{
		using var context = _dbFactory.CreateDbContext();

		var objective = await context.Objectives
			.Include(x => x.Family)
			.Where(x => x.Family.Name == familyName && x.Name == objectiveName)
			.SingleOrDefaultAsync()
			.ConfigureAwait(false);

		if (objective is not null)
		{
            objective.IsHidden = isHidden;
        }
		else
		{
			var family = await GetOrCreateFamily(context, familyName).ConfigureAwait(false);
			var newObjective = new Objective { IsHidden = isHidden, Name = objectiveName, Family = family };
			context.Objectives.Add(newObjective);
		}

        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task UpsertObjectiveFavoriteState(string familyName, string objectiveName, bool isFavorite)
    {
		using var context = _dbFactory.CreateDbContext();

		var objective = await context.Objectives
			.Include(x => x.Family)
			.Where(x => x.Family.Name == familyName && x.Name == objectiveName)
			.SingleOrDefaultAsync()
			.ConfigureAwait(false);

		if (objective is not null)
		{
			objective.IsFavorite = isFavorite;
		}
		else
		{
			var family = await GetOrCreateFamily(context, familyName).ConfigureAwait(false);
			var newObjective = new Objective { IsFavorite = isFavorite, Name = objectiveName, Family = family };
			context.Objectives.Add(newObjective);
		}

		await context.SaveChangesAsync().ConfigureAwait(false);
	}

    private async Task<Family> GetOrCreateFamily(RagnaContext context, string name)
	{
		var family = await context.Families.SingleOrDefaultAsync(x => x.Name == name).ConfigureAwait(false);

		if (family is null)
		{
			context.Families.Add(new Family { Name = name });
			await context.SaveChangesAsync().ConfigureAwait(false);
			family = await context.Families.SingleAsync(x => x.Name == name).ConfigureAwait(false);
        }

		return family;
	}

    private async Task<Objective> GetOrCreateObjective(RagnaContext context, string name, bool isHidden, Family family)
    {
        var objective = await context.Objectives.SingleOrDefaultAsync(x => x.Name == name).ConfigureAwait(false);

        if (objective is null)
        {
            context.Objectives.Add(new Objective { Name = name, IsHidden = isHidden, Family = family });
            await context.SaveChangesAsync().ConfigureAwait(false);
            objective = await context.Objectives.SingleAsync(x => x.Name == name).ConfigureAwait(false);
        }

        return objective;
    }
}
