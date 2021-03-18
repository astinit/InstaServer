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
        public static void InitEngine()
        {
            Engine = EngineFactory.Create(new EngineOptions.Builder().Build());
            SetCookies(new BrowserCookie()).GetAwaiter().GetResult();
            //Engine.Network.StartTransactionHandler
            //    = new Handler<StartTransactionParameters, StartTransactionResponse>(p =>
            //    {
            //        IEnumerable<IHttpHeader> headers = p.Headers;
            //        List<HttpHeader> newHttpHeaders = headers.Cast<HttpHeader>().ToList();
            //        var oldCookie = newHttpHeaders.FirstOrDefault(h => h.Name.ToLower() == "cookie");
            //        if(oldCookie != null)
            //        {
            //            newHttpHeaders.Remove(oldCookie);
            //        }
            //        newHttpHeaders.Add(new HttpHeader("Cookie", $@"{Constants.Ig_did}=A00206A3-496F-4C8A-B46E-259C4055F803; 
            //            {Constants.Mid}=YCLmhAALAAHndx2hFLh7qBRvkP5d; 
            //            {Constants.Ig_nrcb}=1;
            //            {Constants.Shbid}=2379;
            //            {Constants.Rur}=PRN; 
            //            {Constants.Shbts}=1613247768.5875483;
            //            {Constants.Csrftoken}=c7eI4P8H7b7xuUDcIABb8uw62s5pL8cH; 
            //            {Constants.Ds_user_id}=1678886083;  
            //            {Constants.SessionId}="));
            //        return StartTransactionResponse.OverrideHeaders(newHttpHeaders);
            //    });
        }

        public static async Task<CookieUpdateResult> SetCookies(BrowserCookie cookie)
        {
            var result = new CookieUpdateResult();
            foreach (var prop in cookie.GetType().GetProperties())
            {
                result = await SetCookie(prop.Name.ToLower(), (string)prop.GetValue(cookie));
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
