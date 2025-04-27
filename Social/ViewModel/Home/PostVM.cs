namespace Social_App.ViewModel.Home
{
    public class PostVM
    {
        public string? content { get; set; }    
        public IFormFile? image { get; set; } // Nullable to allow for posts without images
    }
}
