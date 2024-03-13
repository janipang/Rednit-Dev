using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;

namespace RednitDev.Controllers;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client;
using RednitDev.Models;
using RednitDev.Services;

public class DiscoverController : Controller
{
    private ManagerService _Manager;

    public DiscoverController(ManagerService managerService)
    {
        _Manager = managerService;
    }

    public IActionResult Index()
    {
        List<Post> posts = GetPosts();
        List<Post> FeedPosts = new List<Post>();

        bool state = User.Identity.IsAuthenticated;
        Console.WriteLine("Cookie state: " + @User.Identity.IsAuthenticated);
        ViewBag.state = state;

        for (int i = 0; i < 3; i++)
        {
            FeedPosts.Add(posts[i]);
        }
        HttpContext.Session.SetInt32("NumberOfFeedPost", 3);
        TempData["FilteredPosts"] = "";

        Console.WriteLine(FeedPosts.Count);
        return View(FeedPosts);
    }

    public IActionResult SearchResult()
    {
        List<Post> posts = null;
        if (TempData["FilteredPosts"] == null)
        {
            posts = new List<Post>();
        }
        else
        {
            posts = JsonSerializer.Deserialize<List<Post>>(TempData["FilteredPosts"] as string);
        }

        List<Post> FeedPosts = new List<Post>();
        Console.WriteLine("posts.count = " + posts.Count);
        for (int i = 0; i < 3 && i < posts.Count; i++)
        {
            FeedPosts.Add(posts[i]);
        }
        HttpContext.Session.SetInt32("NumberOfFeedPost", 3);
        return View(FeedPosts);
    }

    [HttpGet]
    public IActionResult GetMorePosts()
    {
        List<Post> morePosts = new List<Post>();
        int NumberOfFeedPost = (int)HttpContext.Session.GetInt32("NumberOfFeedPost");
        List<Post> posts = null;

        if (TempData["FilteredPosts"] == null || TempData["FilteredPosts"] == "")
        {
            posts = GetPosts();
        }
        else
        {
            posts = JsonSerializer.Deserialize<List<Post>>(TempData["FilteredPosts"] as string);
        }

        Console.WriteLine("post.count == " + posts.Count);

        for (int i = 0; i < 3 && NumberOfFeedPost < posts.Count; i++)
        {
            morePosts.Add(posts[NumberOfFeedPost]);
            NumberOfFeedPost++;
        }

        HttpContext.Session.SetInt32("NumberOfFeedPost", NumberOfFeedPost);

        var html = "";
        foreach (Post post in morePosts)
        {
            html += Components.PostViewComponent.GetViewComponent(post);
        }

        Console.WriteLine(NumberOfFeedPost);

        if (NumberOfFeedPost >= posts.Count)
        {
            html += "a";
        }

        return Json(html);
    }

    public IActionResult ViewPost(string id)
    {
        Console.WriteLine("ViewPost -> " + HttpContext.Session.GetInt32("CurrentCommentId"));
        int Id = int.Parse(id);
        Post post = GetPost(Id);
        bool state = User.Identity.IsAuthenticated;
        Console.WriteLine("Cookie state: " + @User.Identity.IsAuthenticated);
        ViewBag.state = state;
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
        Comment newComment = new Comment { Content = detail };
        currentPost.Comments.Add(newComment);

        UpdatePost(currentPost);

        HttpContext.Session.SetInt32("CurrentCommentId", -1);
        return RedirectToAction(
            "ViewPost",
            "Discover",
            new { id = HttpContext.Session.GetInt32("CurrentPostId") }
        );
    }

    public IActionResult ReplyComment(string detail)
    {
        Console.WriteLine("ReplyComment -> " + HttpContext.Session.GetInt32("CurrentCommentId"));

        int id = (int)HttpContext.Session.GetInt32("CurrentPostId");
        int commentId = (int)HttpContext.Session.GetInt32("CurrentCommentId");
        Post currentPost = GetPost(id);
        Comment newComment = new Comment { Content = detail };
        currentPost.Comments[commentId].Reply = newComment;

        UpdatePost(currentPost);

        HttpContext.Session.SetInt32("CurrentCommentId", -1);
        return RedirectToAction(
            "ViewPost",
            "Discover",
            new { id = HttpContext.Session.GetInt32("CurrentPostId") }
        );
    }

    [HttpPost]
    public IActionResult SetCommentID(string id)
    {
        int Id = int.Parse(id);
        HttpContext.Session.SetInt32("CurrentCommentId", Id);
        Console.WriteLine("Set to " + HttpContext.Session.GetInt32("CurrentCommentId"));
        return RedirectToAction(
            "ViewPost",
            "Discover",
            new { id = HttpContext.Session.GetInt32("CurrentPostId") }
        );
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

        return RedirectToAction(
            "ViewPost",
            "Discover",
            new { id = HttpContext.Session.GetInt32("CurrentPostId") }
        );
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
        return RedirectToAction(
            "ViewPost",
            "Discover",
            new { id = HttpContext.Session.GetInt32("CurrentPostId") }
        );
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
        return RedirectToAction(
            "ViewPost",
            "Discover",
            new { id = HttpContext.Session.GetInt32("CurrentPostId") }
        );
    }

    public IActionResult SearchKeyword(string key)
    {
        List<Post> posts = GetPosts();
        List<Post> filteredPost = new List<Post>();
        foreach (Post post in posts)
        {
            if (post.Detail.Header == "Anntonia Porsild")
            {
                Console.WriteLine(post.Detail.Header.Contains(key) + "  ->  " + key);
            }
            if (
                post.Detail.Header.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0
                || post.Detail.Intro.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0
            )
            {
                filteredPost.Add(post);
            }
        }
        Console.WriteLine("search for " + key + " found " + filteredPost.Count);
        string filteredPostsJson = JsonSerializer.Serialize<List<Post>>(
            filteredPost,
            new JsonSerializerOptions()
        );
        HttpContext.Session.SetString("FilteredPosts", filteredPostsJson);
        return RedirectToAction("Index", "Discover", new { posts = filteredPost });
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
        return RedirectToAction(
            "ViewPost",
            "Discover",
            new { id = HttpContext.Session.GetInt32("CurrentPostId") }
        );
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
        }
        ;
        return posts;
    }

    public static Post GetPost(int id)
    {
        List<Post> posts = GetPosts();
        foreach (Post post in posts)
        {
            if (post.Id == id)
            {
                return post;
            }
        }
        return null;
    }

    public static void UpdatePost(Post edittedPost)
    {
        List<Post> posts = GetPosts();
        for (int i = 0; i < posts.Count; i++)
        {
            if (posts[i].Id == edittedPost.Id)
            {
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
        }
        ;
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
        string username = HttpContext.Request.Cookies["username"]!;
        bool state = @User.Identity.IsAuthenticated;
        ViewBag.state = username;
        ViewBag.state = state;

        return View();
    }

    [HttpPost]
    public IActionResult CreatePost(
        string header,
        string tag,
        string intro,
        string detail,
        string place,
        int memberMax,
        string dateType,
        int startDay,
        int startMonth,
        int startYear,
        int endDay,
        int endMonth,
        int endYear,
        int closeDay,
        int closeMonth,
        int closeYear,
        string requestType,
        string visibility
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
        int idGenerator = posts.Count > 0 ? posts.Max(post => post.Id) + 1 : 1;

        Console.WriteLine("last post id now:" + idGenerator);

        //get account from username
        Account? CurrentAccount = null;
        string username = HttpContext.Session.GetString("username")!;
        foreach (var account in accounts)
        {
            if (username == account.Username)
            {
                CurrentAccount = account;
                break;
            }
        }

        Post newpost = new Post
        {
            Id = idGenerator,
            Author = CurrentAccount,
            Detail = new PostDetail
            {
                Header = header,
                Tag = new List<string> { tag, tag, tag },
                Intro = intro,
                Detail = detail,
                Place = place
            },
            EventDate = new EventDate()
            {
                DateType = dateType,
                Start = new DateOnly(startYear, startMonth, startDay),
                End = dateType == "multiple" ? new DateOnly(endYear, endMonth, endDay) : null,
                CloseSubmit = new DateOnly(closeYear, closeMonth, closeDay),
            },
            Requesting = requestType == "request",
            Visible = visibility == "public",
            MemberCount = 0,
            MemberMax = memberMax,
            DayLeft = (int)
                (new DateTime(startYear, startMonth, startDay) - DateTime.Today).TotalDays,
        };
        posts.Add(newpost);

        var serializeOption = new JsonSerializerOptions();
        serializeOption.WriteIndented = true;
        string jsondata = JsonSerializer.Serialize<List<Post>>(posts, serializeOption);
        Console.WriteLine(jsondata);
        System.IO.File.WriteAllText("./Datacenter/post.json", jsondata);

        return RedirectToAction("Index", "Discover");
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
