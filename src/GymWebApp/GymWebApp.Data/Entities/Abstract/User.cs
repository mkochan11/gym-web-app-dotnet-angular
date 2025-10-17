using System.ComponentModel.DataAnnotations;    

namespace GymWebApp.Data.Entities.Abstract
{
    public abstract class User : BaseEntity
    {
        [Required]
        [MaxLength(450)]
        public string AccountId { get; set; } = string.Empty;
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
