namespace Social_App.ViewModel.Home
{
    public class PostVM
    {
        public string  postId { get; set; } // Unique identifier for the post

        public string UserId { get; set; } // Unique identifier for the user
        public string? content { get; set; }    
        public IFormFile? image { get; set; } // Nullable to allow for posts without images
        public IFormFile? video { get; set; }
    }
}
