using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace E_Library.Helpers
{
    public class AppUser : IdentityUser
    {
        [Required]
        public string FirstName { get;set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        public string LoginHint { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
