using System.ComponentModel.DataAnnotations;

namespace RagnaRoute.Entities;

public class Profile
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;
}
