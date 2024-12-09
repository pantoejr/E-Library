using E_Library.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Library.Models
{
    public class GroupUser : AuditTrail
    {
        [Key]
        public int Id { get; set; }
        public int GroupID { get;set; }
        [ForeignKey(nameof(GroupID))]
        public virtual Group? Group { get; set; }

        public string? UserID { get; set; }
        [ForeignKey(nameof(UserID))]
        public virtual AppUser? AppUser { get; set; }

    }
}
