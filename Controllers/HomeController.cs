using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;
using System.IO;
using System.Text.Json;
using System.Linq.Expressions;

namespace RednitDev.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        // string username = HttpContext.Session.GetString("username")!;
        // string state = HttpContext.Session.GetString("state")!;
        // Console.WriteLine("Session state: " + state);
        string username = HttpContext.Request.Cookies["username"]!;
        bool state = @User.Identity.IsAuthenticated;
        Console.WriteLine("Cookie state: " + @User.Identity.IsAuthenticated);
        ViewBag.state = username;
        ViewBag.state = state;
        
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
        List<Post> hotposts = posts.Take(2).ToList();
    
        return View(hotposts);
    }
}