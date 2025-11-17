using System;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;
using RednitDev.Services;

namespace RednitDev.Components{
    public class FavPostViewComponent: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Post post)
        {
            return View(post);
        }
    }
}