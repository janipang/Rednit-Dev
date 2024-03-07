using RednitDev.Models;
using System.IO;
using System.Text.Json;

namespace RednitDev.Services
{
    public class AccountServiceImpl : AccountService
    {
        private List<Account> account;
        public AccountServiceImpl()
        {
            var accountsJson = System.IO.File.ReadAllText("./Datacenter/user.json"); //Byte Stream
            try
            {
                account = JsonSerializer.Deserialize<List<Account>>(accountsJson); // can read 
            }
            catch (JsonException)
            {
                account = new List<Account>();
            }
        }
        public Account Authenticate(string username, string password)
        {
            return account.SingleOrDefault(a => a.Username == username && a.Password == password);
        }


        public Account CanAuthenticate(string username, string email)
        {
            return account.SingleOrDefault(a => a.Username == username || a.Email == email);
        }

        public Account AddAccount(string username, string email, string password)
        {
            var newAccount = new Account()
            {
                Username = username,
                Password = password,
                Email = email
            };
            account.Add(newAccount);
            var serializeOption = new JsonSerializerOptions();
            serializeOption.WriteIndented = true;
            string jsonData = JsonSerializer.Serialize<List<Account>>(account, serializeOption);
            System.IO.File.WriteAllText("./Datacenter/user.json", jsonData);
            return newAccount;
        }
    }
}