using E_Library.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Library.Models
{
    public class BookAccessLog
    {
        [Key]
        public int Id { get; set; }
        public int BookID { get; set; }
        [ForeignKey(nameof(BookID))]
        public virtual Book? Book { get; set; }
        public string? UserID { get; set; }
        [ForeignKey(nameof(UserID))]
        public virtual AppUser? User { get; set; }
        public DateTime AccessedOn { get; set; }

        [Required]
        public string Action { get; set; } = string.Empty;
    }
}
