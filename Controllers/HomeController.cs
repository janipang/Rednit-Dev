using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;
using RednitDev.Services;

namespace RednitDev.Controllers;

public class HomeController : Controller
{
    private ManagerService _Manager;
    public HomeController(ManagerService managerService)
    {
        _Manager = managerService;
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
        _Manager.UpdateTimeForPost(); //update dayleft evetime that have get /home
        // string username = HttpContext.Session.GetString("username")!;
        // string state = HttpContext.Session.GetString("state")!;
        // Console.WriteLine("Session state: " + state);
        string username = HttpContext.Request.Cookies["username"]!;
        bool state = User.Identity.IsAuthenticated;
        Console.WriteLine("Cookie state: " + @User.Identity.IsAuthenticated);
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
        }
        ;
        List<Post> hotposts = posts.Take(2).ToList();

        return View(hotposts);
    }

    public bool AddToFav(int postId)
    {
        string username = HttpContext.Request.Cookies["username"]!;
        bool state = User.Identity.IsAuthenticated;
        ViewBag.state = state;
        bool isLiked;

        Account userAccount = _Manager.GetAccountByUsername(username);
        User user = _Manager.GetUserByUsername(username);
        Post post = _Manager.GetPostById(postId);

        if (user.Profile.InterestedPosts.Contains(postId)) //user dind't like this post yet
        {
            user.Profile.InterestedPosts.Remove(postId);
            isLiked = false;
            Console.WriteLine("remove post " + postId + " from fav");
        }
        else //dislike
        {
            user.Profile.InterestedPosts.Add(postId);
            isLiked = true;
            Console.WriteLine("add post " + postId + " to fav");
        }
        _Manager.UpdateUser(user);

        return (isLiked);
    }
}
