using System.ComponentModel.DataAnnotations;

namespace RagnaRoute.Entities;
public class Objective
{
    [Key]
    public int Id { get; set; }

    [Required]
    public Profile Profile { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public bool IsHidden { get; set; }
}
