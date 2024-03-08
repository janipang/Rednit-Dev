using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;

namespace RednitDev.Controllers;

public class FavController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

}