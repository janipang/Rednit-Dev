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
        /*-------------------------------------*/
        var postsjson = System.IO.File.ReadAllText("./Datacenter/post.json");
        List<Post> posts;
        try
        {
            posts = JsonSerializer.Deserialize<List<Post>>(postsjson)!; //! is for not to warning me,jezz
        }
        catch (JsonException)
        {
            posts = new List<Post>();
        };
        List<Post> favposts = posts.Take(4).ToList();
    
        return View(favposts);
    }

}