using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace RagnaRoute.Entities;

public class Completion
{
    [Key]
    public int CompletionId { get; set; }

    [Required]
    public Family Family { get; set; } = null!;

    [Required]
    public Objective Objective { get; set; } = null!;

    [Required]
    public Instant CompletionTime { get; set; }
}
