using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;
using System.IO;
using System.Text.Json;
using System.Linq.Expressions;

namespace RednitDev.Controllers;

public class DiscoverController : Controller
{
    public IActionResult Index()
    {
        List<Post> posts = new List<Post>();
        var post = new Post();
        post.Header = "Amanda Obdam";
        post.Tag = "beauty";
        post.Intro = "who is the gorgeous girl";
        post.Detail = "hello everyone I love thsilsnd for all the time";
        post.Place = "Ladphrao, BKK";
        post.request = true;
        post.memberCount = 5;
        post.dayLeft = 15;
        posts.Add(post);
        posts.Add(post);
        posts.Add(post);

        return View(posts);
    }

    public IActionResult CreatePost()
    {
        return View();
    }

    [HttpPost]
    public IActionResult CreatePost(string header, string tag, string intro, string detail, string place)
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

        Post newpost = new Post();
        newpost.Header = header;
        newpost.Tag = tag;
        newpost.Intro = intro;
        newpost.Detail = detail;
        newpost.Place = place;
        newpost.request = true;
        newpost.memberCount = 5;
        newpost.dayLeft = 3;
        posts.Add(newpost);

        var serializeOption = new JsonSerializerOptions();
        serializeOption.WriteIndented = true;
        string jsondata = JsonSerializer.Serialize<List<Post>>(posts, serializeOption);
        Console.WriteLine(jsondata);
        System.IO.File.WriteAllText("./Datacenter/post.json", jsondata);

        return View(newpost);
    }
}
