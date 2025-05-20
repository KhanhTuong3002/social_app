using System.ComponentModel.DataAnnotations;

namespace Social_App.ViewModel.Settings
{
    public class ProfileVm
    {
        [Required(ErrorMessage = "Full Name is required")]
        [RegularExpression(@"^(?!.*\b[aA][dD][mM][iI][nN]\b)(?!.*\s{2,})[\p{L}\s]+$", ErrorMessage = "FullName must contain only letters (accents allowed), numbers, single spaces, and must not contain the word 'admin'.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "UserName is required")]
        [RegularExpression(@"^(?!.*\b[aA][dD][mM][iI][nN]\b)[a-zA-Z0-9._-]+$", ErrorMessage = "UserName can only contain letters, numbers, dots (.), dashes (-), underscores (_) and cannot contain the word 'admin'.")]
        public string UserName { get; set; }
        public string Bio { get; set; }
    }
}
