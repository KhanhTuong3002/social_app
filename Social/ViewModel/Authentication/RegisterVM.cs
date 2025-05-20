using System.ComponentModel.DataAnnotations;

namespace Social_App.ViewModel.Authentication
{
    public class RegisterVM
    {
        [Required(ErrorMessage ="First Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First Name must be between 2 and 50 characters.")]
        [RegularExpression(@"^(?!.*\b[aA][dD][mM][iI][nN]\b)(?!.*\s{2,})[\p{L}\s]+$", ErrorMessage = "First Name can only contain letters and cannot contain 'admin'.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last Name must be between 2 and 50 characters.")]
        [RegularExpression(@"^(?!.*\b[aA][dD][mM][iI][nN]\b)(?!.*\s{2,})[\p{L}\s]+$", ErrorMessage = "First Name can only contain letters and cannot contain 'admin'.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Max lenght 8 digits include uppercase, lowercase, number, and special character ")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }
}
