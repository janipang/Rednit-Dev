using System;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Controllers;
using RednitDev.Models;

namespace RednitDev.Components{
    public class ViewOthersPostDetailViewComponent: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Post post)
        {
            for(int i = 0; i < post.Comments.Count; i++){
                post.Comments[i].Id = (uint)i;
            }
            string username = HttpContext.Request.Cookies["username"];
            Account myAccount = DiscoverController.GetAccount(username);
            ViewBag.HaveJoined = DiscoverController.HaveJoined(post, myAccount);
            return View(post);
        }
    }
}
