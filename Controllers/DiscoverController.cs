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
        // List<Post> posts = new List<Post>();
        // posts.Add(new Post{
        //     Detail = new PostDetail{
        //         Header = "หาคนทำสมุดระบายสีแจกเด็ก",
        //         Intro = "ต้องการเพื่อนเข้าร่วมกลุ่มอีก 4 คน ขอคนตั้งใจทำงาน ขยัน ไม่อู้ รักศิลปะ ไม่ใช้เอไอในการเจนรูป"
        //     },
        //     Requesting = false,
        //     MemberCount = 3,
        //     MemberMax = 4,
        //     DayLeft = 1
        //     });
        // posts.Add(new Post{
        //     Detail = new PostDetail{
        //         Header = "หาเพื่อนไปเที่ยวภูเก็ตสิ้นเดือน",
        //         Intro = "ขอคนชอบช้อปปิ้ง เดินเก่ง เที่ยวเก่ง ที่พักและการเดินทางจะจัดเตรียมให้ค่ะ ไม่เอาคนกลัวแดดนะคะ ขอลุย ๆ ค่ะ"
        //     },
        //     Requesting = true,
        //     MemberCount = 5,
        //     MemberMax = 8,
        //     DayLeft = 2
        //     });
        return View(posts);
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