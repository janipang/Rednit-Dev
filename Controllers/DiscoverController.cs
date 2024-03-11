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

        return View();
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

        //get account from username
        Account CurrentAccount;
        string username = HttpContext.Session.GetString("username")!;
        foreach(var account in accounts){
            if (username == account.Username){
                CurrentAccount = account;
                break;
            }
        };

        Post newpost = new Post
        {
            Author = new User{
                // AccountSetter = CurrentAccount
            },
            Detail = new PostDetail
            {
                Header = header,
                Tag = [tag,tag,tag],
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
