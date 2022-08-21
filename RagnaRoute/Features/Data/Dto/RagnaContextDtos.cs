using NodaTime;

namespace RagnaRoute.Data;

public record QuestCompletionDto(string Family, string Name, Instant CompletionTime);