using DotNetBrowser.Cookies;
using InstaServer.BLL;
using InstaServer.BLL.Helpers;
using InstaServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InstaServer.Controllers
{
    public class BrowserCookieController : Controller
    {
        private readonly ILogger<BrowserCookieController> _logger;

        public BrowserCookieController(ILogger<BrowserCookieController> logger) => _logger = logger;

        public IActionResult Edit() => View(BrowserManager.BrowserCookie);

        [HttpPost]
        public async Task<IActionResult> Edit(BrowserCookie browserCookie)
        {
            var result = await BrowserManager.SetCookies(browserCookie);
            if (result.Exception == null)
            {
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
                _logger.LogError(JsonConvert.SerializeObject(result.Exception));
            }
            return View(browserCookie);
        }
    }
}
