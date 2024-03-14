using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;
using System.IO;
using System.Text.Json;

namespace RednitDev.Controllers;

public class NotiController : Controller
{
    public IActionResult Index()
    {

        // bool state = User.Identity.IsAuthenticated;
        // Console.WriteLine("Cookie state: " + @User.Identity.IsAuthenticated);
        // ViewBag.state = state;

        var username =  HttpContext.Request.Cookies["username"]; 
        var usersJson = System.IO.File.ReadAllText("./Datacenter/User.json"); //Byte Stream
        var postsJson = System.IO.File.ReadAllText("./Datacenter/post.json"); //Byte Stream
        var users = new List<User>();
        var posts = new List<Post>();
        try
        {
            users = JsonSerializer.Deserialize<List<User>>(usersJson); // can read 
            posts = JsonSerializer.Deserialize<List<Post>>(postsJson); // can read 
        }
        catch (JsonException)
        {
            users = new List<User>();
            posts = new List<Post>();
        }
        var user = new User();
        if (users != null)
        {
            user = users.SingleOrDefault(a => a.Account.Username == username);
        }
        // ดึง noti 
        List<Noti> userNoti = user.Noti; // ฝั่ง post ส่งมาให้ ผั่งแป้ง
        var noti = new List<ShowNoti>{}; // noti ที่รุจน์จะต้องเอาไปแสดง 
        var requestedNoti = new List<ShowRequestedNoti>{};
        foreach(var n in userNoti)
        {
            var post = posts.SingleOrDefault(a => a.Id == n.IdPost);
            var header = post.Detail.Header;
            var  email = user.Account.Email;
            string  text = "";
            if (n.Type == "Success")
            {
                text += "Email contact : " + email;
            }
            else  if (n.Type == "Failed")
            {
                text = "Sorry, can't join.";
            }
            if (n.Type != "Request")
            {
                var temp = new ShowNoti();
                temp.Header = header;
                temp.Type = n.Type;
                temp.Text = text;
                temp.IdPost = n.IdPost;
                noti.Add(temp);
            }
            else  if (n.Type == "Request")
            {
                var temp2 = new ShowRequestedNoti();
                var requestedUser = users.SingleOrDefault(a => a.Account.Username == n.WhoRequest.Username);
                temp2.Img = requestedUser.Profile.Image;
                temp2.Header = header;
                temp2.Type = n.Type;
                temp2.IdPost = n.IdPost;
                temp2.WhoRequest =n.WhoRequest.Username;
                requestedNoti.Add(temp2);
            }

        }
        ViewBag.requestedNoti = requestedNoti;
        ViewBag.noti = noti;
        return View();
    }

}