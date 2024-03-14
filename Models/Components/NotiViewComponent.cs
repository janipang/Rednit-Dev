using System;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;

namespace RednitDev.Components{
    public class NotiViewComponent: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(ShowNoti noti)
        {
            return View(noti);
        }
    }
}