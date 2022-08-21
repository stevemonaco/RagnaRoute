using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace RagnaRoute.Entities;

public class QuestCompletion
{
    public int Id { get; set; }

    [Required]
    public string ObjectiveFamily { get; set; } = null!;

    [Required]
    public string ObjectiveName { get; set; } = null!;

    [Required]
    public Instant CompletionTime { get; set; }
}
