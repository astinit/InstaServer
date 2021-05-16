using DotNetBrowser.Cookies;
using InstaServer.BLL;
using InstaServer.BLL.Helpers;
using InstaServer.Logging;
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

        public BrowserCookieController(ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile(Constants.LogPath);
            _logger = loggerFactory.CreateLogger<BrowserCookieController>();
        }

        public IActionResult Edit() => View(BrowserManager.BrowserCookie);

        [HttpPost]
        public async Task<IActionResult> Edit(BrowserCookie browserCookie)
        {
            var result = await BrowserManager.SetCookiesAsync(browserCookie);
            if (result.Exception == null)
            {
                try
                {
                    await BrowserManager.WriteCookieToFile(browserCookie);
                    TempData["Success"] = result.Message;
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    TempData["Error"] = result.Message;
                }
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
