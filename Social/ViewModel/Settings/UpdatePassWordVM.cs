using System.ComponentModel.DataAnnotations;

namespace Social_App.ViewModel.Settings
{
    public class UpdatePassWordVM
    {
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "New Password must have 8 digits include uppercase, lowercase, number, and special character ")]
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
