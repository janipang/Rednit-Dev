using RednitDev.Models;

namespace RednitDev.Services
{
    public interface AccountService
    {
        public Account Authenticate(string username, string password);
        public Account CanAuthenticate(string username, string email);
        public Account AddAccount(string username, string email, string password);
    }
}