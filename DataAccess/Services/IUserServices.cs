using BussinessObject.Entities;

namespace DataAccess.Services
{
    public interface IUserServices
    {
        Task<User> GetUser(string loggedInUserId);
        Task UpdateUserProfile(string loggedInUserId, string avatarUrl);

        Task<List<Post>> GetUserPosts(string userId);

    }
}