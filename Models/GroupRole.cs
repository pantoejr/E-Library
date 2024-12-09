using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Library.Models
{
    public class GroupRole
    {
        [Key]
        public int Id { get; set; }
        public int GroupID { get; set; }
        [ForeignKey(nameof(GroupID))]
        public virtual Group? Group { get; set; }

        public string? RoleID { get; set; }
        [ForeignKey(nameof(RoleID))]
        public virtual IdentityRole? IdentityRole { get;set; }
    }
}
