using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;

namespace RednitDev.Controllers;

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
    public IActionResult CreatePost(string header, string tag, string title, string detail, string place)
    {
        ViewBag.header = header;
        ViewBag.tag = tag;
        ViewBag.title = title;
        ViewBag.detail = detail;
        ViewBag.place = place;

        Post newpost = new Post();
        newpost.Header = header;
        newpost.Tag = tag;
        newpost.Title = title;
        newpost.Detail = detail;
        newpost.Place = place;
        Console.WriteLine(header + "\n" + tag + "\n" + title + "\n");
        return View(newpost);
    }
}
