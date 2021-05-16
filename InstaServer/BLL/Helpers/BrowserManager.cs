using InstaServer.BLL.Interfaces;
using System.Threading.Tasks;
using System;
using DotNetBrowser.Engine;
using DotNetBrowser.Browser;
using DotNetBrowser.Dom;
using DotNetBrowser.Cookies;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using InstaServer.Models;
using InstaServer.Logging;
using System.IO;
using DotNetBrowser.Handlers;
using DotNetBrowser.Net.Handlers;
using DotNetBrowser.Net;
using System.Collections.Generic;

namespace InstaServer.BLL.Helpers
{
    public class BrowserManager
    {
        private readonly string _url;
        public static BrowserCookie BrowserCookie { get; private set; }

        public static IEngine Engine { get; set; }
        public IBrowser Browser { get; private set; }
        public static async Task InitEngineAsync()
        {
            Engine = EngineFactory.Create(new EngineOptions.Builder().Build());
            BrowserCookie = new BrowserCookie();
            FileInfo fileInf = new FileInfo("cookie.json");
            if (fileInf.Exists)
            {
                BrowserCookie = await ReadCookieFromFile() ?? BrowserCookie;
                await SetCookiesAsync(BrowserCookie);
            }
        }

        public static async Task WriteCookieToFile(BrowserCookie cookie)
        {
            string json = JsonConvert.SerializeObject(cookie);
            using var sw = new StreamWriter("cookie.json", false, System.Text.Encoding.Default);
            await sw.WriteLineAsync(json);
        }

        private static async Task<BrowserCookie> ReadCookieFromFile()
        {
            string json;
            using (var sr = new StreamReader("cookie.json"))
            {
                json = await sr.ReadToEndAsync();
            }
            return JsonConvert.DeserializeObject<BrowserCookie>(json);
        }

        public static async Task<CookieUpdateResult> SetCookiesAsync(BrowserCookie cookie)
        {
            var result = new CookieUpdateResult();
            foreach (var prop in cookie.GetType().GetProperties())
            {
                var value = prop.GetValue(cookie) as string ?? "";
                result = await SetCookie(prop.Name.ToLower(), value);
            }
            BrowserCookie = cookie;
            return result;
        }
        public static async Task<CookieUpdateResult> SetCookie(string name, string value)
        {
            var result = new CookieUpdateResult();
            Cookie cookie = new Cookie.Builder
            {
                Name = name,
                Value = value,
                DomainName = ".instagram.com",
                ExpirationTime = DateTime.Now.AddYears(1),
                Path = "/"
            }.Build();
            var sessionIdCookie = (await Engine.CookieStore.GetAllCookies()).FirstOrDefault(c => c.Name == name);
            try
            {
                bool deleted = true;
                if (sessionIdCookie != null)
                {
                    deleted = await Engine.CookieStore.Delete(sessionIdCookie);
                }
                var seted = await Engine.CookieStore.SetCookie("https://www.instagram.com", cookie);
                if (!deleted || !seted)
                {
                    result.Message = "Error. Changes not saved";
                }
                else
                {
                    Engine.CookieStore.Flush();
                    result.Message = "Changes saved successfully";
                }
            }
            catch (Exception)
            {
                result.Message = "Error. Changes not saved";
            }
            result.Message = "Changes saved successfully";
            return result;
        }

        public BrowserManager(IParserSettings settings)
        {
            Browser = Engine.CreateBrowser();
            _url = settings.Url;
            //Browser = new ChromiumWebBrowser(_url);
        }

        public async Task<IDocument> GetDocument()
        {
            try
            {
                await Browser.Navigation.LoadUrl(_url);
            }
            catch (Exception ex)
            {
                var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole()).AddFile(Constants.LogPath);
                var logger = loggerFactory.CreateLogger<BrowserManager>();
                logger.LogError(JsonConvert.SerializeObject(ex));
                logger.LogInformation(_url);
            }
            return Browser.MainFrame.Document;
        }

        //public async Task<IDocument> GetDocument()
        //{
        //    try
        //    {
        //        var f = await Browser.EvaluateScriptAsync("document");
        //    }
        //    catch (Exception ex)
        //    {
        //        var loggerFactory = LoggerFactory.Create(builder => builder.AddEventLog());
        //        var logger = loggerFactory.CreateLogger<BrowserManager>();
        //        logger.LogError(JsonConvert.SerializeObject(ex));
        //        logger.LogInformation(_url);
        //    }
        //    return Browser.MainFrame.Document;
        //}
    }
}
