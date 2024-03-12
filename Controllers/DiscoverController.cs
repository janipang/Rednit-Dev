using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;
using System.IO;
using System.Text.Json;
using System.Linq.Expressions;

namespace RednitDev.Controllers;
using RednitDev.Models;

public class DiscoverController : Controller
{
    public IActionResult Index()
    {
        List<Post> posts = GetPosts();
        return View(posts);
    }

    public IActionResult ViewPost(string id)
    {
        Console.WriteLine("ViewPost -> " + HttpContext.Session.GetInt32("CurrentCommentId"));
        int Id = int.Parse(id);
        Post post = GetPost(Id);
        HttpContext.Session.SetInt32("CurrentPostId", Id);
        return View(post);
    }

    [HttpPost]
    public IActionResult AddComment(string detail)
    {
        Console.WriteLine("AddComment -> " + HttpContext.Session.GetInt32("CurrentCommentId"));
        if (HttpContext.Session.GetInt32("CurrentCommentId") >= 0)
        {
            return RedirectToAction("ReplyComment", "Discover", new { detail = detail });
        }

        int id = (int)HttpContext.Session.GetInt32("CurrentPostId");
        Post currentPost = GetPost(id);
        Comment newComment = new Comment
        {
            Content = detail
        };
        currentPost.Comments.Add(newComment);

        UpdatePost(currentPost);

        HttpContext.Session.SetInt32("CurrentCommentId", -1);
        return RedirectToAction("ViewPost", "Discover", new { id = HttpContext.Session.GetInt32("CurrentPostId") });
    }

    public IActionResult ReplyComment(string detail)
    {
        Console.WriteLine("ReplyComment -> " + HttpContext.Session.GetInt32("CurrentCommentId"));
        
        int id = (int)HttpContext.Session.GetInt32("CurrentPostId");
        int commentId = (int)HttpContext.Session.GetInt32("CurrentCommentId");
        Post currentPost = GetPost(id);
        Comment newComment = new Comment
        {
            Content = detail
        };
        currentPost.Comments[commentId].Reply = newComment;

        UpdatePost(currentPost);

        HttpContext.Session.SetInt32("CurrentCommentId", -1);
        return RedirectToAction("ViewPost", "Discover", new { id = HttpContext.Session.GetInt32("CurrentPostId") });
    }

    [HttpPost]
    public IActionResult SetCommentID(string id)
    {
        int Id = int.Parse(id);
        HttpContext.Session.SetInt32("CurrentCommentId", Id);
        Console.WriteLine("Set to " + HttpContext.Session.GetInt32("CurrentCommentId"));
        return RedirectToAction("ViewPost", "Discover", new { id = HttpContext.Session.GetInt32("CurrentPostId") });
    }

    [HttpPost]
    public IActionResult JoinEvent()
    {
        int postId = (int)HttpContext.Session.GetInt32("CurrentPostId");
        string username = HttpContext.Request.Cookies["username"];
        Post currentPost = GetPost(postId);
        Account account = GetAccount(username);
        if (!HaveJoined(currentPost, account))
        {
            currentPost.Joined.Add(account);
        }
        Console.WriteLine(username + " has joined post " + postId);

        UpdatePost(currentPost);

        return RedirectToAction("ViewPost", "Discover", new { id = HttpContext.Session.GetInt32("CurrentPostId") });
    }

    [HttpPost]
    public IActionResult SendRequest()
    {
        int postId = (int)HttpContext.Session.GetInt32("CurrentPostId");
        string username = HttpContext.Request.Cookies["username"];
        Post currentPost = GetPost(postId);
        Account account = GetAccount(username);
        if (!HaveJoined(currentPost, account) && !HaveRequested(currentPost, account))
        {
            currentPost.Requested.Add(account);
        }
        Console.WriteLine(username + " send request to post " + postId);
        UpdatePost(currentPost);
        return RedirectToAction("ViewPost", "Discover", new { id = HttpContext.Session.GetInt32("CurrentPostId") });
    }

    public IActionResult AcceptRequest(string _username)
    {
        int postId = (int)HttpContext.Session.GetInt32("CurrentPostId");
        Account account = GetAccount(_username);
        Post currentPost = GetPost(postId);
        Console.WriteLine("Accept " + _username);
        Console.WriteLine(account);
        if (!HaveJoined(currentPost, account))
        {
            currentPost.Joined.Add(account);
            currentPost.Requested.Remove(account);
        }
        Console.WriteLine(_username + " was added to post " + postId);
        UpdatePost(currentPost);
        return RedirectToAction("ViewPost", "Discover", new { id = HttpContext.Session.GetInt32("CurrentPostId") });
    }

    public IActionResult DeclineRequest(string _username)
    {
        int postId = (int)HttpContext.Session.GetInt32("CurrentPostId");
        Account account = GetAccount(_username);
        Post currentPost = GetPost(postId);
        Console.WriteLine("Decline " + _username);
        Console.WriteLine(account);
        currentPost.Requested.Remove(account);
        Console.WriteLine(_username + " was rejected from post " + postId);
        UpdatePost(currentPost);
        return RedirectToAction("ViewPost", "Discover", new { id = HttpContext.Session.GetInt32("CurrentPostId") });
    }

    public static List<Post> GetPosts()
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
        };
        return posts;
    }

    public static Post GetPost(int id){
        List<Post> posts = GetPosts();
        foreach(Post post in posts){
            if(post.Id == id){
                return post;
            }
        }
        return null;
    }

    public static void UpdatePost(Post edittedPost){
        List<Post> posts = GetPosts();
        for(int i = 0; i < posts.Count; i++){
            if(posts[i].Id == edittedPost.Id){
                posts[i] = edittedPost;
                break;
            }
        }
        var serializeOption = new JsonSerializerOptions();
        serializeOption.WriteIndented = true;
        string jsondata = JsonSerializer.Serialize<List<Post>>(posts, serializeOption);
        System.IO.File.WriteAllText("./Datacenter/post.json", jsondata);
    }

    public static List<Account> GetAccounts()
    {
        var accountsJson = System.IO.File.ReadAllText("./Datacenter/account.json");
        List<Account> accounts;
        try
        {
            accounts = JsonSerializer.Deserialize<List<Account>>(accountsJson)!;
        }
        catch (JsonException)
        {
            accounts = new List<Account>();
        };
        return accounts;
    }

    public static Account GetAccount(string username)
    {
        List<Account> accounts = GetAccounts();
        foreach (Account account in accounts)
        {
            if (account.Username == username)
            {
                return account;
            }
        }
        return null;
    }

    public static bool HaveJoined(Post post, Account myAccount)
    {
        foreach (Account account in post.Joined)
        {
            if (account.Equals(myAccount))
            {
                return true;
            }
        }
        return false;
    }

    public static bool HaveRequested(Post post, Account myAccount)
    {
        foreach (Account account in post.Requested)
        {
            if (account.Equals(myAccount))
            {
                return true;
            }
        }
        return false;
    }
    public IActionResult CreatePost()
    {
        return View();
    }

    [HttpPost]
    public IActionResult CreatePost(
        string header, string tag, string intro, string detail, string place,
        int memberMax, string dateType,
        int startDay, int startMonth, int startYear,
        int endDay, int endMonth, int endYear,
        int closeDay, int closeMonth, int closeYear,
        string requestType, string visibility
    )
    {
        var postsjson = System.IO.File.ReadAllText("./Datacenter/post.json");
        var accountsjson = System.IO.File.ReadAllText("./Datacenter/account.json");
        List<Post> posts;
        try
        {
            posts = JsonSerializer.Deserialize<List<Post>>(postsjson)!; //! is for not to warning me,jezz
        }
        catch (JsonException)
        {
            posts = new List<Post>();
        }

        List<Account> accounts;
        try
        {
            accounts = JsonSerializer.Deserialize<List<Account>>(accountsjson)!; //! is for not to warning me,jezz
        }
        catch (JsonException)
        {
            accounts = new List<Account>();
        }

        Post newpost = new Post
        {
            Author = new User
            {
                AccountSetter = accounts[0]
            },
            Detail = new PostDetail
            {
                Header = header,
                Tag = [tag, tag, tag],
                Intro = intro,
                Detail = detail,
                Place = place
            },
            EventDate = new EventDate()
            {
                DateType = dateType,
                Start = new DateOnly(startYear, startMonth, startDay),
                End = new DateOnly(endYear, endMonth, endDay),
                CloseSubmit = new DateOnly(closeYear, closeMonth, closeDay),
            },
            Requesting = requestType == "request", //(request,open)
            Visible = visibility == "public", //(public,draft)
            MemberCount = 5,
            MemberMax = memberMax,
            DayLeft = 3,
        };
        posts.Add(newpost);

        var serializeOption = new JsonSerializerOptions();
        serializeOption.WriteIndented = true;
        string jsondata = JsonSerializer.Serialize<List<Post>>(posts, serializeOption);
        Console.WriteLine(jsondata);
        System.IO.File.WriteAllText("./Datacenter/post.json", jsondata);

        return View();
    }
}