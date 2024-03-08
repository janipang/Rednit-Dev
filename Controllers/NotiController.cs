using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;

namespace RednitDev.Controllers;

public class NotiController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

}