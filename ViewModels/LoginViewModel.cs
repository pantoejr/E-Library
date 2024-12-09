using System.ComponentModel.DataAnnotations;

namespace E_Library.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email cannot be empty")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password cannot be empty")]
        [DataType(DataType.Password, ErrorMessage = "Username or Password is Incorrect")]
        public string Password { get; set; } = string.Empty;
    }
}
