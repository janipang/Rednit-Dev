using System;
using Microsoft.AspNetCore.Mvc;
using ann.Models;

namespace ann.Components{
    public class PostViewComponent: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Post post)
        {
            return View(post);
        }
    }
}