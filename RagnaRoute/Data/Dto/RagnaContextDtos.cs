using NodaTime;

namespace RagnaRoute.Data;

public record CompletionDto(string FamilyName, string ObjectiveName, Instant CompletionTime);
public record ObjectiveStateDto(string FamilyName, string ObjectiveName, Instant? LastCompletion, bool IsHidden, bool IsFavorite);