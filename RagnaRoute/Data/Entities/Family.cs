using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace RagnaRoute.Entities;

[Index(nameof(Name), IsUnique = true)]
public class Family
{
    [Key]
    public int FamilyId { get; set; }

    [Required]
    public string Name { get; set; } = null!;
}
