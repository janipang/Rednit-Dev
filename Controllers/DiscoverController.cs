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
using System.Data;
using Microsoft.CodeAnalysis.CodeStyle;
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
        List<Post> posts = GetActivePosts();
        List<Post> FeedPosts = new List<Post>();

        bool state = User.Identity.IsAuthenticated;
        Console.WriteLine("Cookie state: " + @User.Identity.IsAuthenticated);
        ViewBag.state = state;

       for(int i = 0 ; i < 3; i++){
            FeedPosts.Add(posts[i]);
        }
        HttpContext.Session.SetInt32("NumberOfFeedPost", 3);
        HttpContext.Session.SetString("FilteredPosts", "");
        ViewBag.HasMorePost = true;

        List<string> topSearch = GetTopSearch();
        ViewBag.TopSearch = new List<HotSearch>();
        foreach(var e in topSearch){
            List<HotSearch> hotSearches = GetHotSearch();
            foreach(var h in hotSearches){
                if(h.Keyword == e){
                    ViewBag.TopSearch.Add(h);
                    break;
                }
            }
        }
        Console.WriteLine(FeedPosts.Count);
        return View(FeedPosts);
    }

    public IActionResult SearchResult()
    {
        List<Post> posts = null;
        if(HttpContext.Session.GetString("FilteredPosts") == ""){
            posts = new List<Post>();
            Console.WriteLine("nulll");
        }
        else{
            posts = JsonSerializer.Deserialize<List<Post>>(HttpContext.Session.GetString("FilteredPosts"));
            Console.WriteLine("post.count " + posts.Count);
        }

        List<Post> FeedPosts = new List<Post>();
        Console.WriteLine("posts.count = " + posts.Count);
        for (int i = 0; i < 3 && i < posts.Count; i++)
        {
            FeedPosts.Add(posts[i]);
        }
        HttpContext.Session.SetInt32("NumberOfFeedPost", 3);
        if(HttpContext.Session.GetString("SearchedTag") != ""){
            ViewBag.SearchTag = HttpContext.Session.GetString("SearchedTag");
        }
        else{
            ViewBag.SearchTag = "";
        }
        ViewBag.HasMorePost = posts.Count > FeedPosts.Count;

        List<string> topSearch = GetTopSearch();
        ViewBag.TopSearch = new List<HotSearch>();
        foreach(var e in topSearch){
            List<HotSearch> hotSearches = GetHotSearch();
            foreach(var h in hotSearches){
                if(h.Keyword == e){
                    ViewBag.TopSearch.Add(h);
                    break;
                }
            }
        }
        return View(FeedPosts);
    }

    [HttpGet]
    public IActionResult GetMorePosts()
    {
        List<Post> morePosts = new List<Post>();
        int NumberOfFeedPost = (int)HttpContext.Session.GetInt32("NumberOfFeedPost");
        List<Post> posts = null;

        if(HttpContext.Session.GetString("FilteredPosts") == null || HttpContext.Session.GetString("FilteredPosts") == ""){
            posts = GetActivePosts(); 
        }
        else{
            posts = JsonSerializer.Deserialize<List<Post>>(HttpContext.Session.GetString("FilteredPosts"));
        }

        Console.WriteLine("post.count == " + posts.Count);

        for (int i = 0; i < 3 && NumberOfFeedPost < posts.Count; i++)
        {
            Console.WriteLine(posts[NumberOfFeedPost].Author.Username);
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
        Console.WriteLine("ViewPost -> " + HttpContext.Session.GetInt32("CurrentPostId"));
        Console.WriteLine("post id = " + id);
        int Id = int.Parse(id);
        Post post = GetPost(Id);
        bool state = User.Identity.IsAuthenticated;
        Console.WriteLine("Cookie state: " + @User.Identity.IsAuthenticated);
        ViewBag.state = state;
        HttpContext.Session.SetInt32("CurrentPostId", Id);

        int userId = (int)HttpContext.Session.GetInt32("Id");
        User user = GetUser(userId);

        if(user == null){
            ViewBag.PostType = 3;
        }
        else if(user.Account.Equals(post.Author)){
            if(post.Requesting){
                ViewBag.PostType = 1;
            }
            else{
                ViewBag.PostType = 0;
            }
        }
        else{
            if(post.Requesting){
                ViewBag.PostType = 3;
            }
            else{
                ViewBag.PostType = 2;
            }
        }
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
        int userId = (int)HttpContext.Session.GetInt32("Id");
        User user = GetUser(userId);
        Post currentPost = GetPost(id);

        Comment newComment = new Comment
        {
            User = user,
            Date = DateTime.Now,
            Content = detail
        };

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
            if(currentPost.Joined.Count == currentPost.MemberMax){
                currentPost.Active = false;
            }
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

    public IActionResult AcceptRequest(int userId)
    {
        int postId = (int)HttpContext.Session.GetInt32("CurrentPostId");
        User user = GetUser(userId);
        Account account = user.Account;
        Post currentPost = GetPost(postId);
        Console.WriteLine("Accept " + account.Username);
        Console.WriteLine(account);
        if (!HaveJoined(currentPost, account))
        {
            currentPost.Joined.Add(account);
            currentPost.Requested.Remove(account);

            Noti notification = new Noti{
                Type = "Success",
                IdPost = postId,
                WhoRequest = null 
            };

            Console.WriteLine("send success noti to" + account.Username + " " + user);
            user.Noti.Add(notification);
            UpdateUser(user);

            if(currentPost.Joined.Count == currentPost.MemberMax){
                SendRejectNotificationToOthers(currentPost, postId);
                currentPost.Active = false;
            }
        }
        Console.WriteLine(account.Username + " was added to post " + postId);
        UpdatePost(currentPost);
        return RedirectToAction(
            "ViewPost",
            "Discover",
            new { id = HttpContext.Session.GetInt32("CurrentPostId") }
        );
    }

    public void SendRejectNotificationToOthers(Post post, int postId){
        for(int i = 0; i < post.Requested.Count; i++){
            User user = GetUser(post.Requested[i].Username);
            Console.WriteLine("send reject noti to" + post.Requested[i].Username + " " + user);
            Noti notification = new Noti{
                Type = "Failed",
                IdPost = postId,
                WhoRequest = null 
            };
            user.Noti.Add(notification);
            UpdateUser(user);
        }

        post.Requested = new List<Account>();
    }

    public IActionResult DeclineRequest(int userId)
    {
        Debug.WriteLine("=== = == = = = ====");
        int postId = (int)HttpContext.Session.GetInt32("CurrentPostId");
        User user = GetUser(userId);
        Account account = user.Account;
        Post currentPost = GetPost(postId);
        Console.WriteLine("Decline " +account.Username);
        Console.WriteLine(account);
        currentPost.Requested.Remove(account);
        
        Noti notification = new Noti{
            Type = "Failed",
            IdPost = postId,
            WhoRequest = null
        };
        user.Noti.Add(notification);
        UpdateUser(user);
        Console.WriteLine(account.Username + " was rejected from post " + postId);
        UpdatePost(currentPost);
        return RedirectToAction("ViewPost", "Discover", new { id = HttpContext.Session.GetInt32("CurrentPostId") });
    }

    public IActionResult SearchKeyword(string key){
        List<Post> posts = GetActivePosts();
        List<Post> filteredPost = new List<Post>();
        foreach(Post post in posts){
            if(
                post.Detail.Header.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0 || 
                post.Detail.Intro.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0
            ){
                filteredPost.Add(post);
            }
        }
        Console.WriteLine("search for " + key + " found " + filteredPost.Count);
        AddToHotSearch(key);
        string filteredPostsJson = JsonSerializer.Serialize<List<Post>>(
            filteredPost,
            new JsonSerializerOptions()
        );
        HttpContext.Session.SetString("FilteredPosts", filteredPostsJson);
        HttpContext.Session.SetString("SearchedTag", "");
        return RedirectToAction("SearchResult", "Discover");
    }

    public IActionResult SearchByTag(string tag){
        List<Post> posts = GetActivePosts();
        List<Post> filteredPost = new List<Post>();
        foreach(Post post in posts){
            foreach(var _tag in post.Detail.Tag){
                if(_tag.IndexOf(tag, StringComparison.OrdinalIgnoreCase) >= 0){
                    filteredPost.Add(post);
                }
            }
        }
        Console.WriteLine("search for tag" + tag + " found " + filteredPost.Count);
        string filteredPostsJson = JsonSerializer.Serialize<List<Post>>(filteredPost, new JsonSerializerOptions());
        HttpContext.Session.SetString("FilteredPosts", filteredPostsJson);
        HttpContext.Session.SetString("SearchedTag", tag);
        return RedirectToAction("SearchResult", "Discover");
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

    public static List<Post> GetActivePosts(){
        List<Post> posts = GetPosts();
        List<Post> activePosts = new List<Post>();
        foreach(Post post in posts){
            if(post.Active){
                activePosts.Add(post);
            }
        } 
        return activePosts;
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

    public static List<User> GetUsers(){
        var usersJson = System.IO.File.ReadAllText("./Datacenter/User.json");
        List<User> users;
        try
        {
            users = JsonSerializer.Deserialize<List<User>>(usersJson)!;
        }
        catch (JsonException)
        {
            users = new List<User>();
        };
        return users;
    }

    public static User GetUser(string username){
        List<User> users = GetUsers();
        foreach(User user in users){
            if(user.Account.Username == username){
                return user;
            }
        }
        return null;
    }

    public static User GetUser(int userId){
        List<User> users = GetUsers();
        foreach(User user in users){
            if(user.Id == userId){
                return user;
            }
        }
        return null;
    }

    public static void UpdateUser(User edittedUser){
        List<User> users = GetUsers();
        for(int i = 0; i < users.Count; i++){
            if(users[i].Id == edittedUser.Id){
                users[i] = edittedUser;
                break;
            }
        }
        var serializeOption = new JsonSerializerOptions();
        serializeOption.WriteIndented = true;
        string jsondata = JsonSerializer.Serialize<List<User>>(users, serializeOption);
        System.IO.File.WriteAllText("./Datacenter/user.json", jsondata);
    }

    public List<string> GetTopSearch(){
        List<HotSearch> hotSearch = GetHotSearch();
        List<string> searched = new List<string>();
        for(int i = 0 ;i < 5; i++){
            string keyword = GetHighestSearch(searched);
            searched.Add(keyword);
        }
        return searched;
    }

    public string GetHighestSearch(List<string> searched){
        List<HotSearch> hotSearch = GetHotSearch();
        int max = 0;
        string keyword = "";
        foreach(var e in hotSearch){
            if(!searched.Contains(e.Keyword)){
                int cnt = e.Count;
                if(cnt > max){
                    max = cnt;
                    keyword = e.Keyword;
                }
            }
        }
        return keyword;
    }
    public static void AddToHotSearch(string keyword){
        List<HotSearch> hotSearch = GetHotSearch();
        bool added = false;
        foreach(var e in hotSearch){
            if(e.Keyword == keyword){
                e.Count++;
                added = true;
            }
        }
        if(!added){
            hotSearch.Add(new HotSearch{Keyword = keyword, Count = 1});
        }
        var serializeOption = new JsonSerializerOptions();
        serializeOption.WriteIndented = true;
        string jsondata = JsonSerializer.Serialize<List<HotSearch>>(hotSearch, serializeOption);
        System.IO.File.WriteAllText("./Datacenter/hotSearch.json", jsondata);
    }

    public static List<HotSearch> GetHotSearch(){
        var hotsearchJson = System.IO.File.ReadAllText("./Datacenter/hotSearch.json");
        List<HotSearch> hotSearch;
        try
        {
            hotSearch = JsonSerializer.Deserialize<List<HotSearch>>(hotsearchJson)!;
        }
        catch (JsonException)
        {
            hotSearch = new List<HotSearch>();
        };
        return hotSearch;
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
                Tag = tag.Split(", ").ToList(),
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

    public IActionResult AddTag(string tag){
        return Json(TagString(tag));
    }

    public string TagString(string tag){

        return $"""
        <div class="tagElement">{tag}</div>
        """;
    }
}

