using System.ComponentModel.DataAnnotations;

namespace E_Library.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
