using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;
using System.IO;
using System.Text.Json;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Drawing;

namespace RednitDev.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult ChooseTag()
    {
        return View();
    }
    public IActionResult Setting()
    {        
        return View();
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

    public IActionResult AddInterestedTag(string tag1, string tag2, string tag3)
    {
        string username = HttpContext.Request.Cookies["username"];
        Console.WriteLine("username " + username);
        User user = DiscoverController.GetUser(username);
        Console.WriteLine("user " + user);
        if (tag1 != "") {
            Console.WriteLine("tag1");
            user.Profile.InterestedTag.Add(tag1);
        }
        if (tag2 != "") {
            Console.WriteLine("tag2");
            user.Profile.InterestedTag.Add(tag2);
        }
        if (tag3 != "") {
            Console.WriteLine("tag3");
            user.Profile.InterestedTag.Add(tag3);
        }
        DiscoverController.UpdateUser(user);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult EditProfile(string username, string bio, string newPass, string confirmPass, string image) {
        string realUsername = HttpContext.Request.Cookies["username"];
        Console.WriteLine("realUsername " + realUsername);
        User user = DiscoverController.GetUser(realUsername);
        Console.WriteLine("user " + user);
        Console.WriteLine(username + bio + newPass + confirmPass + image);
        if (newPass != null && newPass != "" && confirmPass != "" && confirmPass != null) {
            if (newPass == confirmPass) {
                user.Account.Password = newPass;
            }
        }
        if (image != null && image != "") {
            user.Profile.Image = image;
        }
        if (username != null && username != "") {
            user.Account.Username = username;
            
        }
        if (bio != null && bio != "") {
            user.Profile.Caption = bio;
        }
        
        DiscoverController.UpdateUser(user);
        return RedirectToAction("Setting", "Home");
    }

    // public async void changeUsername(string newUsername){
    //     httpContextAccessor.HttpContext.Session.SetString("username", username);
    // }
    
}