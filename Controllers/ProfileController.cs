using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Security;
using System.ComponentModel;




namespace RednitDev.Controllers;
using static Microsoft.AspNetCore.Mvc.ViewComponent;
using RednitDev.Components;
using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client;


public class ProfileController : Controller
{
    public User onlineUser;
    [Authorize]
    public IActionResult MyProfile()
    {
        // ดึง username
        string username = HttpContext.Request.Cookies["username"] ?? "No data";
        Console.WriteLine("User: " + username);

        //อ่านไฟล์ user 
        var usersJson = System.IO.File.ReadAllText("./Datacenter/user.json");

        List<User> users;
        try
        {
            users = JsonSerializer.Deserialize<List<User>>(usersJson); //! is for not to warning me,jezz
        }
        catch (JsonException)
        {
            users = new List<User>();
        };

        Console.WriteLine("users: " + users.Count);

        var user = users.SingleOrDefault(x => x.Account.Username == username);
        onlineUser = user;

        Console.WriteLine("User: +" + user.Account.Email);
        Console.WriteLine("RequesingPosts: +" + user.Profile.RequesingPosts);
        ViewBag.user = user;

        return View();
    }
    
    public IActionResult ViewProfile(Account account, User user)
    {
        return View();
    }

}