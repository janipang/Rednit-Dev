using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;

namespace RednitDev.Services
{
    public class ManagerServiceImpl : ManagerService
    {
        public Post? GetPostById(int id)
        {
            var postsjson = System.IO.File.ReadAllText("./Datacenter/post.json");
            List<Post> posts;
            try
            {
                posts = JsonSerializer.Deserialize<List<Post>>(postsjson)!; //! is for not to warning me,jezz
            }
            catch (JsonException)
            {
                posts = new List<Post>();
            }
            foreach (Post post in posts)
            {
                if (post.Id == id)
                {
                    return post;
                }
            }
            return null;
        }

        public Account? GetAccountByUsername(string username)
        {
            var accountsjson = System.IO.File.ReadAllText("./Datacenter/account.json");
            List<Account> accounts;
            try
            {
                accounts = JsonSerializer.Deserialize<List<Account>>(accountsjson)!; //! is for not to warning me,jezz
            }
            catch (JsonException)
            {
                accounts = new List<Account>();
            }
            foreach (Account account in accounts)
            {
                if (account.Username == username)
                {
                    return account;
                }
            }
            return null;
        }

        public User? GetUserByUsername(string username)
        {
            var usersjson = System.IO.File.ReadAllText("./Datacenter/User.json");
            List<User> users;
            try
            {
                users = JsonSerializer.Deserialize<List<User>>(usersjson)!; //! is for not to warning me,jezz
            }
            catch (JsonException)
            {
                users = new List<User>();
            }
            foreach (User user in users)
            {
                if (user.Account.Username == username)
                {

                    return user;
                }
            }
            return null;
        }

        public void UpdateTimeForPost()
        {
            var postsjson = System.IO.File.ReadAllText("./Datacenter/post.json");
            List<Post> posts;
            try
            {
                posts = JsonSerializer.Deserialize<List<Post>>(postsjson)!; //! is for not to warning me,jezz
            }
            catch (JsonException)
            {
                Console.WriteLine("erreo");
                posts = new List<Post>();
            }

            foreach (Post post in posts)
            {
                post.DayLeft = (
                    post.EventDate.Start.ToDateTime(new TimeOnly()) - DateTime.Today
                ).Days;
            }

            var serializeOption = new JsonSerializerOptions();
            serializeOption.WriteIndented = true;
            string jsondata = JsonSerializer.Serialize<List<Post>>(posts, serializeOption);
            System.IO.File.WriteAllText("./Datacenter/post.json", jsondata);
        }

        public void UpdateMemberForPost()
        {
            var postsjson = System.IO.File.ReadAllText("./Datacenter/post.json");
            List<Post> posts;
            try
            {
                posts = JsonSerializer.Deserialize<List<Post>>(postsjson)!; //! is for not to warning me,jezz
            }
            catch (JsonException)
            {
                posts = new List<Post>();
            }
            Console.WriteLine(posts);
            foreach (Post post in posts)
            {
                post.MemberCount = post.Joined.Count;
            }

            var serializeOption = new JsonSerializerOptions();
            serializeOption.WriteIndented = true;
            string jsondata = JsonSerializer.Serialize<List<Post>>(posts, serializeOption);
            System.IO.File.WriteAllText("./Datacenter/post.json", jsondata);
        }

        public void UpdateUser(User updatedUser)
        {
            var usersjson = System.IO.File.ReadAllText("./Datacenter/User.json");
            List<User> users;
            try
            {
                users = JsonSerializer.Deserialize<List<User>>(usersjson)!; //! is for not to warning me,jezz
            }
            catch (JsonException)
            {
                users = new List<User>();
            }

            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Account.Username == updatedUser.Account.Username)
                {
                    users[i] = updatedUser;
                    break;
                }
            }

            var serializeOption = new JsonSerializerOptions();
            serializeOption.WriteIndented = true;
            string jsondata = JsonSerializer.Serialize<List<User>>(users, serializeOption);
            System.IO.File.WriteAllText("./Datacenter/user.json", jsondata);
        }
    }
}
