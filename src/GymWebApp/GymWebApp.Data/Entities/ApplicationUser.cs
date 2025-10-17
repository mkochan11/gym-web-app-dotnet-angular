using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GymWebApp.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Surname { get; set; } = string.Empty;
        
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        
        [MaxLength(200)]
        public string? Address { get; set; }
        
        public DateTime? DateOfBirth { get; set; }
        
        [MaxLength(10)]
        public string? Gender { get; set; }
    }
}
