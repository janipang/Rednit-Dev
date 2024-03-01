using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;

namespace RednitDev.Controllers;

public class DiscoverController : Controller
{
    private readonly ILogger<DiscoverController> _logger;

    public DiscoverController(ILogger<DiscoverController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
}
