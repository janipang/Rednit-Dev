using System;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;

namespace RednitDev.Components{
    public class CommentInMyPostViewComponent: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Comment comment)
        {
            return View(comment);
        }
    }
}
