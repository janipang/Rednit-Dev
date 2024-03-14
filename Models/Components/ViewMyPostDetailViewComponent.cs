using System;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Controllers;
using RednitDev.Models;
using RednitDev.Services;

namespace RednitDev.Components
{
    public class ViewMyPostDetailViewComponent : ViewComponent
    {
        private ManagerService _Manager;
        public ViewMyPostDetailViewComponent(ManagerService managerService)
        {
            _Manager = managerService;
        }
        public async Task<IViewComponentResult> InvokeAsync(Post post)
        {
            for (int i = 0; i < post.Comments.Count; i++)
            {
                post.Comments[i].Id = (uint)i;
            }
            ViewBag.CurrentCommentId = HttpContext.Session.GetInt32("CurrentCommentId");
            string username = post.Author.Username;
            User user = DiscoverController.GetUser(username);
            ViewBag.ProfileImage = user.Profile.Image;

            ViewBag.JoinedImage = new List<string>();
            foreach(var account in post.Joined){
                User _user = _Manager.GetUserByUsername(account.Username);
                ViewBag.JoinedImage.Add(_user.Profile.Image);
                Console.WriteLine("img -> " + _user.Profile.Image);
            }

            return View(post);
        }
    }
}
