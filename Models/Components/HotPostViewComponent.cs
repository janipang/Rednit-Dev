using System;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;

namespace RednitDev.Components{
    public class HotPostViewComponent: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Post post)
        {
            return View(post);
        }
    }
}