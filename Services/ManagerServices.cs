using RednitDev.Models;

namespace RednitDev.Services
{
    public interface ManagerService
    {
        public Post? GetPostById(int id);
        public Account? GetAccountByUsername(string username);
        public User? GetUserByUsername(string username);
        public void UpdateTimeForPost();
        public void UpdateMemberForPost();
        public void UpdateUser(User updatedUser);
        public void ReplacePost(int postId, Post newpost);
        public void DeletePost(int postId);
    }
}