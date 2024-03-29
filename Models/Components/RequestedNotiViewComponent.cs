using System;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;

namespace RednitDev.Components{
    public class RequestedNotiViewComponent: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(ShowRequestedNoti requestedNoti)
        {
            return View(requestedNoti);
        }
    }
}