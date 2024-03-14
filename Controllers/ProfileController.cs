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

    [Authorize]
    public IActionResult MyProfile()
    {
        bool state = User.Identity.IsAuthenticated;
        ViewBag.state = state;
        // ดึง username
        int idUser = (int)HttpContext.Session.GetInt32("Id");

        var user = GetUser(idUser);

        Console.WriteLine("User: +" + user.Account.Email);
        Console.WriteLine("RequesingPosts: +" + user.Profile.RequesingPosts);
        ViewBag.user = user;

        return View();
    }
    
    public IActionResult ViewProfile(int idUser)
    {
        bool state = User.Identity.IsAuthenticated;
        ViewBag.state = state;
        var user = GetUser(idUser);

        Console.WriteLine(" View User: +" + user.Account.Email);
        Console.WriteLine("RequesingPosts: +");
        ViewBag.user = user;
        return View();
    }
    public static User GetUser(int idUser)
    {
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
        var user = users.SingleOrDefault(x => x.Id == idUser);
        return user;
    }
}