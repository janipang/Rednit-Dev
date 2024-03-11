using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;
using System.IO;
using System.Text.Json;
using System.Linq.Expressions;

namespace RednitDev.Controllers;
using RednitDev.Models;

public class PostDetailController : Controller
{
    public IActionResult Index()
    {
        Post post = new Post{
            Detail = new PostDetail{
                Header = "หาคนทำสมุดระบายสีแจกเด็ก",
                Intro = "ต้องการเพื่อนเข้าร่วมกลุ่มอีก 4 คน ขอคนตั้งใจทำงาน ขยัน ไม่อู้ รักศิลปะ ไม่ใช้เอไอในการเจนรูป"
            },
            Requesting = false,
            MemberCount = 3,
            MemberMax = 4,
            DayLeft = 1
            };
        //post = ViewBag.Parameter;
        return View(post);
    }
}