using System;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;

namespace RednitDev.Components
{
    public class ViewMyPostDetailViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Post post)
        {
            for (int i = 0; i < post.Comments.Count; i++)
            {
                post.Comments[i].Id = (uint)i;
            }
            return View(post);
        }
    }
}
