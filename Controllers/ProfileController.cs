using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace RednitDev.Controllers;

public class ProfileController : Controller
{
    [Authorize]
    public IActionResult MyProfile()
    {
        // ดึง username
        string username = HttpContext.Session.GetString("username") ?? "No data";
        Console.WriteLine("User: " + username);

        //อ่านไฟล์ user 
        var usersJson = System.IO.File.ReadAllText("./Datacenter/User.json");
        List<User> users = JsonSerializer.Deserialize<List<User>>(usersJson);
        Console.WriteLine("Users: " + users);
        var user = users.SingleOrDefault(x => x.Account.Username == username);
        Console.WriteLine("User: " + user.Account.Username);
        //ส่งข้อมูล user ให้ไปแสดงออก 
        ViewBag.Username = user.Account.Username;
        ViewBag.Caption = user.Profile.Caption;
        ViewBag.CreatedPosts = user.Profile.CreatedPosts;
        var requestPost = new List<Post>{};
        foreach(Post post in user.Profile.JoinningPosts)
        {
            if (post.Requesting)
            {
                requestPost.Append(post);
            }
        }
        ViewBag.RequestPost = requestPost;
        ViewBag.Image = user.Profile.Image;
        ViewBag.InterestedTag = user.Profile.InterestedTag;
        
        return View();
    }

    public IActionResult ViewProfile(Account account, User user)
    {
        return View();
    }

}