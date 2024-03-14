using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using RednitDev.Models;
using RednitDev.Services;
using System.IO;
using System.Text.Json;
namespace RednitDev.Controllers;


public class AccessController : Controller
{
    private readonly AccountService accountService;
    private readonly IHttpContextAccessor httpContextAccessor;
    public AccessController(AccountService _accountService, IHttpContextAccessor _httpContextAccessor)
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
            List<Claim> claims = new List<Claim>() {
                    new Claim(ClaimTypes.NameIdentifier, account.Username),
                    new Claim("OtherProperties", "Example Role")
                };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), properties);
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(1),
                Secure = true,
                HttpOnly = true
            };

            httpContextAccessor.HttpContext.Session.SetString("state", "online");
            httpContextAccessor.HttpContext.Session.SetString("username", account.Username);
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
            return RedirectToAction("Index", "Home");
        }
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Signup(string username, string email, string password)
    {
        var account = accountService.CanAuthenticate(username, email);
        if (account == null)
        {
            var newAccount = accountService.AddAccount(username, email, password);
            List<Claim> claims = new List<Claim>() {
                    new Claim(ClaimTypes.NameIdentifier, newAccount.Username),
                    new Claim("OtherProperties", "Example Role")
                };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), properties);

            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(1),
                Secure = true,
                HttpOnly = true
            };

            httpContextAccessor.HttpContext.Session.SetString("state", "online");
            httpContextAccessor.HttpContext.Session.SetString("username", newAccount.Username);
            HttpContext.Response.Cookies.Append("username", newAccount.Username, cookieOptions);

            return RedirectToAction("Index", "Home");
        }
        ViewData["ValidateMessage"] = "Username or E-mail already in use.";
        return View();
    }
}