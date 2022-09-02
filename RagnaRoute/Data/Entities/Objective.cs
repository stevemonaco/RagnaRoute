using Microsoft.EntityFrameworkCore;
using NodaTime;
using System.ComponentModel.DataAnnotations;

namespace RagnaRoute.Entities;

[Index(nameof(Name), IsUnique = true)]
public class Objective
{
    [Key]
    public int ObjectiveId { get; set; }

    [Required]
    public Family Family { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public bool IsHidden { get; set; }

    public Instant? LastCompletion { get; set; }
}
