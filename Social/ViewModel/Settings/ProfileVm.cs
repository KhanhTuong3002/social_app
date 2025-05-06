using System.ComponentModel.DataAnnotations;

namespace Social_App.ViewModel.Settings
{
    public class ProfileVm
    {
        [Required(ErrorMessage = "Full Name is required")]
        [RegularExpression(@"^(?!.*\b[aA][dD][mM][iI][nN]\b)[a-zA-Z]+$", ErrorMessage = "First Name can only contain letters and cannot be 'admin'.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "UserName is required")]
        [RegularExpression(@"^(?!.*\b[aA][dD][mM][iI][nN]\b)[a-zA-Z0-9]+$", ErrorMessage = "UserName can only contain letters and numbers and cannot be 'admin'.")]
        public string UserName { get; set; }
        public string Bio { get; set; }
    }
}
