using System.ComponentModel.DataAnnotations;

namespace Social_App.ViewModel.Settings
{
    public class ProfileVm
    {
        [Required(ErrorMessage = "Full Name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }
        public string Bio { get; set; }
    }
}
