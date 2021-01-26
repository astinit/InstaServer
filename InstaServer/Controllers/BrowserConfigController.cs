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
    public class BrowserConfigController : Controller
    {
        private readonly ILogger<BrowserConfigController> _logger;

        public BrowserConfigController(ILogger<BrowserConfigController> logger) => _logger = logger;

        public async Task<IActionResult> Edit()
        {
            var sessionId = (await BrowserManager.Engine.CookieStore.GetAllCookies()).FirstOrDefault(c => c.Name == Constants.SessionIdCookieName)?.Value;
            return View(new BrowserConfig() { SessionId = sessionId });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BrowserConfig browserConfig)
        {
            var expirationTime = DateTime.Now;
            expirationTime.AddYears(1);
            Cookie cookie = new Cookie.Builder
            {
                Name = Constants.SessionIdCookieName,
                Value = browserConfig.SessionId,
                DomainName = ".instagram.com",
                ExpirationTime = expirationTime,
                Path = "/"
            }.Build();
            var sessionIdCookie = (await BrowserManager.Engine.CookieStore.GetAllCookies()).FirstOrDefault(c => c.Name == Constants.SessionIdCookieName);
            try
            {
                bool deleted = true;
                if (sessionIdCookie != null)
                {
                    deleted = await BrowserManager.Engine.CookieStore.Delete(sessionIdCookie);
                }
                var seted = await BrowserManager.Engine.CookieStore.SetCookie("https://www.instagram.com", cookie);
                if (!deleted || !seted)
                {
                    TempData["Error"] = "Error. Changes not saved";
                }
                else
                {
                    TempData["Success"] = "Changes saved successfully";
                    BrowserManager.Engine.CookieStore.Flush();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(JsonConvert.SerializeObject(ex));
                TempData["Error"] = "Error. Changes not saved";
            }
            return View(browserConfig);
        }
    }
}
