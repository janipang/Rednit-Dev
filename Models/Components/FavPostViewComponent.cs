using System;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;
using RednitDev.Services;

namespace RednitDev.Components{
    public class FavPostViewComponent: ViewComponent
    {
        private ManagerService _Manager;
        public FavPostViewComponent(ManagerService managerService)
        {
            _Manager = managerService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Post post)
        {
            User user = _Manager.GetUserByUsername(post.Author.Username);
            Console.WriteLine(user.Profile.Image);
            ViewBag.ProfileImage = user.Profile.Image;
            return View(post);
        }
    }
}