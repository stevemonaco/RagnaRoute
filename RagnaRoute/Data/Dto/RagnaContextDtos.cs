using NodaTime;

namespace RagnaRoute.Data;

public record QuestCompletionDto(string Profile, string Name, Instant CompletionTime);