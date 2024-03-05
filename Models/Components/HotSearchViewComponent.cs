using System;
using Microsoft.AspNetCore.Mvc;
using ann.Models;

namespace ann.Components{
    public class HotSearchViewComponent: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}