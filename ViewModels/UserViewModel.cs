using System.ComponentModel.DataAnnotations;

namespace E_Library.ViewModels
{
    public class UserViewModel
    {
        public string UserID { get; set; }
        [DataType(DataType.Text)]
        public string FirstName { get; set; }
        [DataType(DataType.Text)]
        public string LastName { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords don't match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public string? LoginHint { get; set; }
        public int GroupID { get; set; }
    }
}
