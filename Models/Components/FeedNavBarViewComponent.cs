using System;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;

namespace RednitDev.Components{
    public class FeedNavBarViewComponent: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
