using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;
using System.IO;
using System.Text.Json;
using System.Linq.Expressions;
namespace RednitDev.Controllers;

public class FavController : Controller
{
    public IActionResult Index()
    {
        string username = HttpContext.Session.GetString("username")!;
        string state = HttpContext.Session.GetString("state")!;
        Console.WriteLine("state: " + state);
        // ViewBag.Username = username;
        ViewBag.state = state;
        var usersjson = System.IO.File.ReadAllText("./Datacenter/User.json");
        var postsjson = System.IO.File.ReadAllText("./Datacenter/post.json");
        List<Post> posts;
        List<User> users;
        try
        {
            users = JsonSerializer.Deserialize<List<User>>(usersjson)!;
            posts = JsonSerializer.Deserialize<List<Post>>(postsjson)!; //! is for not to warning me,jezz
        }
        catch (JsonException)
        {
            users = new List<User>();
            posts = new List<Post>();
        }
        var user = new User();
        if (users != null) {
            user = users.SingleOrDefault(a => a.Account.Username == username);
        }
        var favposts = new List<Post>{};
        foreach(var x in user.Profile.InterestedPosts){
            var favpost = posts.SingleOrDefault(a => a.Id == x);
            favposts.Add(favpost);
        }
    
        return View(favposts);
    }

}