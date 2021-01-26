using InstaServer.BLL.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using DotNetBrowser.Engine;
using DotNetBrowser.Browser;
using DotNetBrowser.Dom;
using System.Collections.Generic;
using System.Text;
using DotNetBrowser.Handlers;
using DotNetBrowser.Net.Handlers;
using System.Web;
using DotNetBrowser.Net;
using System.Linq;
using DotNetBrowser.Cookies;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace InstaServer.BLL.Helpers
{
    public class BrowserManager
    {
        private readonly string  _url;

        public static IEngine Engine { get; set; }
        public IBrowser Browser { get; private set; }

        public static void InitEngine()
        {
            Engine = EngineFactory.Create(new EngineOptions.Builder
            {
                LicenseKey = "1BNKDJZJSD1S9NASJH2DBLAF1O7LT7J33VKEMHJ3RQZJMEBIHSU7XTQ4A16X3GEC8N8QMV",
                SandboxDisabled = true
            }
            .Build());
            var expirationTime = DateTime.Now;
            expirationTime.AddYears(1);
            Cookie cookie = new Cookie.Builder
            {
                Name = Constants.SessionIdCookieName,
                Value = "1678886083%3AmQuhrIXUGmAU9I%3A18",
                DomainName = ".instagram.com",
                ExpirationTime = expirationTime,
                Path = "/"
            }.Build();
            _ = Engine.CookieStore.SetCookie("https://www.instagram.com", cookie).Result;
            Engine.CookieStore.Flush();
        }

        public BrowserManager(IParserSettings settings)
        {
            Browser = Engine.CreateBrowser();
            _url = settings.Url;
        }

        public async Task<IDocument> GetDocument()
        {
            try
            {
                await Browser.Navigation.LoadUrl(_url);
            }
            catch (Exception ex)
            {
                var loggerFactory = LoggerFactory.Create(builder => builder.AddEventLog());
                var logger = loggerFactory.CreateLogger<BrowserManager>();
                logger.LogError(JsonConvert.SerializeObject(ex));
                logger.LogInformation(_url);
            }
            return Browser.MainFrame.Document;
        }
    }
}
