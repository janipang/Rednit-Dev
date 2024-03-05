using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ann.Models;

namespace ann.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Feed(){
        List<Post> posts = new List<Post>();
        posts.Add(new Post{
            Header = "หาคนทำสมุดระบายสีแจกเด็ก",
            Intro = "ต้องการเพื่อนเข้าร่วมกลุ่มอีก 4 คน ขอคนตั้งใจทำงาน ขยัน ไม่อู้ รักศิลปะ ไม่ใช้เอไอในการเจนรูป",
            request = false,
            memberCount = 3,
            dayLeft = 1
            });
        posts.Add(new Post{
            Header = "หาเพื่อนไปเที่ยวภูเก็ตสิ้นเดือน",
            Intro = "ขอคนชอบช้อปปิ้ง เดินเก่ง เที่ยวเก่ง ที่พักและการเดินทางจะจัดเตรียมให้ค่ะ ไม่เอาคนกลัวแดดนะคะ ขอลุย ๆ ค่ะ",
            request = true,
            memberCount = 5,
            dayLeft = 2
            });
        return View(posts);
    }
}
