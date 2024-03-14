using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;
using RednitDev.Services;
using System.IO;
using System.Text.Json;

namespace RednitDev.Controllers;


public class AccessController : Controller
{

    private readonly AccountService accountService;
    private readonly IHttpContextAccessor httpContextAccessor;

    public AccessController(
        AccountService _accountService,
        IHttpContextAccessor _httpContextAccessor
    )
    {
        accountService = _accountService;
        httpContextAccessor = _httpContextAccessor;
    }


    public IActionResult Login()
    {
        ClaimsPrincipal claimsUser = HttpContext.User;
        if (claimsUser.Identity.IsAuthenticated) // Login   HttpContext.User.Identity.IsAuthenticated
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        // check user
        var account = accountService.Authenticate(username, password);
        if (account != null)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, account.Username),
                new Claim("OtherProperties", "Example Role")
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(1),
                Secure = true,
                HttpOnly = true
            };

            User user = DiscoverController.GetUser(account.Username);

            httpContextAccessor.HttpContext.Session.SetString("state", "online");
            httpContextAccessor.HttpContext.Session.SetString("username", account.Username);
            httpContextAccessor.HttpContext.Session.SetInt32("Id", user.Id);
            Console.WriteLine("id" + httpContextAccessor.HttpContext.Session.GetInt32("Id"));
            Console.WriteLine("user" + account.Username);
            HttpContext.Response.Cookies.Append("username", account.Username, cookieOptions);
            return RedirectToAction("Index", "Home");
        }
        ViewData["ValidateMessage"] = "Username or Password is invalid.";
        return View();
    }
    public async void changeUsername(string newUsername){
        httpContextAccessor.HttpContext.Session.SetString("username", newUsername);
    }

    public async Task<IActionResult> LogOut()
    {
        httpContextAccessor.HttpContext.Session.Remove("username");
        httpContextAccessor.HttpContext.Session.Remove("state");
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Signup()
    {
        ClaimsPrincipal claimsUser = HttpContext.User;
        if (claimsUser.Identity.IsAuthenticated) // Login
        {
            Console.WriteLine("ChooseTag");
            return RedirectToAction("ChooseTag", "Home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Signup(string username, string email, string password)
    {
        Console.WriteLine("Signup");
        var account = accountService.CanAuthenticate(username, email);
        if (account == null)
        {
             var usersJson = System.IO.File.ReadAllText("./Datacenter/user.json"); //Byte Stream
            List<User> users;
            try
            {
                users = JsonSerializer.Deserialize<List<User>>(usersJson); // can read 
            }
            catch (JsonException)
            {
                users = new List<User>();
            }

            var newAccount = accountService.AddAccount(username, email, password);
            // สร้าง User 
            var newUser = new User();
            newUser.Account = newAccount;
            newUser.Profile = new Profile();
            newUser.Id = users.Count;
            Console.WriteLine(newUser.Id);

            // add user in list
            users.Add(newUser);
            var serializeOption = new JsonSerializerOptions();
            serializeOption.WriteIndented = true;
            string jsonData = JsonSerializer.Serialize<List<User>>(users, serializeOption);
            System.IO.File.WriteAllText("./Datacenter/User.json", jsonData);

            //cookies
            List<Claim> claims = new List<Claim>() {
                    new Claim(ClaimTypes.NameIdentifier, newAccount.Username),
                    new Claim("OtherProperties", "Example Role")
                };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(1),
                Secure = true,
                HttpOnly = true
            };

            httpContextAccessor.HttpContext.Session.SetString("state", "online");
            httpContextAccessor.HttpContext.Session.SetString("username", newAccount.Username);
            httpContextAccessor.HttpContext.Session.SetInt32("Id", newUser.Id);
            HttpContext.Response.Cookies.Append("username", newAccount.Username, cookieOptions);
            Console.WriteLine(httpContextAccessor.HttpContext.Session.GetInt32("Id"));

            return RedirectToAction("ChooseTag", "Home");
        }
        ViewData["ValidateMessage"] = "Username or E-mail already in use.";
        return View();
    }
}
