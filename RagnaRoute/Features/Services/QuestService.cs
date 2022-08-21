using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using RagnaRoute.Data;
using RagnaRoute.Entities;

namespace RagnaRoute.Services;
public class QuestService
{
	private readonly IDbContextFactory<RagnaContext> _dbFactory;

	public QuestService(IDbContextFactory<RagnaContext> dbFactory)
	{
		_dbFactory = dbFactory;
	}

	public async Task AddBossQuestCompletion(string objectiveFamily, string objectiveName, Instant completionTime)
	{
		using var context = _dbFactory.CreateDbContext();

		var completion = new QuestCompletion()
		{
			ObjectiveFamily = objectiveFamily,
			ObjectiveName = objectiveName,
			CompletionTime = completionTime
		};

		context.BossQuestCompletions.Add(completion);
		await context.SaveChangesAsync().ConfigureAwait(false);
	}

	public async Task<ICollection<QuestCompletionDto>> GetBossQuestCompletions(string objectiveFamily, string objectiveName)
	{
		using var context = _dbFactory.CreateDbContext();

		return await context.BossQuestCompletions.AsNoTracking()
			.Where(x => x.ObjectiveFamily == objectiveFamily && x.ObjectiveName == objectiveName)
			.Select(x => new QuestCompletionDto(x.ObjectiveFamily, x.ObjectiveName, x.CompletionTime))
			.ToListAsync()
			.ConfigureAwait(false);
	}
}
