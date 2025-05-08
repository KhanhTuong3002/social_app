namespace Social_App.ViewModel.Friends
{
    public class UserWithFirendCountVM
    {
        public string UserId { get; set; }
        public string FullName { get; set; }

        public string? AvatarUrl { get; set; }
        public int FriendCount { get; set; }
        public string FriendCountDiplay  => 
            FriendCount == 0 ? "No follower" :
            FriendCount == 1 ? "1 follower" :
            $"{FriendCount} followers";
    }
}
