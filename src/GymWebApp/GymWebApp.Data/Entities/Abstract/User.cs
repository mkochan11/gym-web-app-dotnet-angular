using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymWebApp.Data.Entities.Abstract;

public abstract class User : BaseEntity
{
    [Required]
    [MaxLength(450)]
    public string AccountId { get; set; } = string.Empty;

    [ForeignKey(nameof(AccountId))]
    public ApplicationUser? ApplicationUser { get; set; }
}
