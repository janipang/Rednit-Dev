using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;
using System.IO;
using System.Text.Json;
using System.Linq.Expressions;
namespace RednitDev.Controllers;

public class ManagerController : Controller
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

    public User? GetUserByusername(string username)
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
            posts = new List<Post>();
        }
        foreach (Post post in posts)
        {
            post.DayLeft = (post.EventDate.Start.ToDateTime(new TimeOnly()) - new DateTime()).Days;
        }
    }
}