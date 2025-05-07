using BussinessObject.Entities;

namespace Social_App.ViewModel.Users
{
    public class GetUserProfileVM
    {
        public User User { get; set; }
        public List<Post> Posts { get; set; }
    }
}
