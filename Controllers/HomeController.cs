using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;
using System.IO;
using System.Text.Json;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Drawing;
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
        bool state = User.Identity.IsAuthenticated;
        Console.WriteLine("Cookie state: " + @User.Identity.IsAuthenticated);
        ViewBag.state = state;
        return View();
    }

    public IActionResult Setting()
    {        
        bool state = User.Identity.IsAuthenticated;
        Console.WriteLine("Cookie state: " + @User.Identity.IsAuthenticated);
        ViewBag.state = state;
        return View();
    }

    public IActionResult Index()
    {
        _Manager.UpdateTimeForPost(); //update dayleft evetime that have get /home
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
        int userId = (int)HttpContext.Session.GetInt32("Id");
        Console.WriteLine("realUsername " + userId);
        User user = DiscoverController.GetUser(userId);
        Console.WriteLine("user " + user);
        Console.WriteLine(username + bio + newPass + confirmPass + image);
        if (newPass != null && newPass != "" && confirmPass != "" && confirmPass != null) {
            if (newPass == confirmPass) {
                user.Account.Password = newPass;
                List<Account> accounts = DiscoverController.GetAccounts();
                accounts[userId].Password = newPass;
                var serializeOption = new JsonSerializerOptions();
                serializeOption.WriteIndented = true;
                string jsondata = JsonSerializer.Serialize<List<Account>>(accounts, serializeOption);
                System.IO.File.WriteAllText("./Datacenter/account.json", jsondata);
            }
        }
        if (image != null && image != "") {
            user.Profile.Image = image;
        }
        if (username != null && username != "") {
            user.Account.Username = username;
            List<Account> accounts = DiscoverController.GetAccounts();
            accounts[userId].Username = username;
            var serializeOption = new JsonSerializerOptions();
            serializeOption.WriteIndented = true;
            string jsondata = JsonSerializer.Serialize<List<Account>>(accounts, serializeOption);
            System.IO.File.WriteAllText("./Datacenter/account.json", jsondata);
        }
        if (bio != null && bio != "") {
            user.Profile.Caption = bio;
        }
        
        DiscoverController.UpdateUser(user);
        return RedirectToAction("Setting", "Home");
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

