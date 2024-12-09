using E_Library.Helpers;
using System.ComponentModel.DataAnnotations;

namespace E_Library.Models
{
    public class Group : AuditTrail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
    }
}
