using E_Library.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Library.Models
{
    public class Book : AuditTrail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CategoryID { get; set; }
        [ForeignKey(nameof(CategoryID))]
        public virtual Category? Category { get; set; }

        [Required]
        public byte[]? CoverImage { get; set; }
        [Required]
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.Now;
        public int Views { get; set; }
    }
}
