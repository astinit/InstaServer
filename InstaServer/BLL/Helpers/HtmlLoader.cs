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

namespace InstaServer.BLL.Helpers
{
    public class HtmlLoader
    {
        private readonly string  _url;

        private static IEngine _engine;
        private readonly IBrowser _browser;

        public static void InitEngine()
        {
            _engine = EngineFactory.Create(new EngineOptions.Builder
            {
                LicenseKey = "1BNKDJZJSD1S9NASJH2DBLAF1O7LT7J33VKEMHJ3RQZJMEBIHSU7XTQ4A16X3GEC8N8QMV",
                SandboxDisabled = true
            }
            .Build());
            _engine.Network.AuthenticateHandler = new Handler<AuthenticateParameters, AuthenticateResponse>(p =>
    AuthenticateResponse.Continue(Constants.UserName, Constants.Password));

            //await _browser.Navigation.LoadUrl(Constants.LoginUrl);
            //if (NeedAuth(_browser.MainFrame.Document))
            //{
            //    await SignIn(_browser.MainFrame.Document);
            //}
        }

        public HtmlLoader(IParserSettings settings)
        {
            _browser = _engine.CreateBrowser();
            _url = settings.Url;
        }

        public async Task<IDocument> GetDocument()
        {
            try
            {
                await _browser.Navigation.LoadUrl(_url);
            }
            catch (Exception ex)
            {
                //TODO Log error
                var f = ex;
            }
            return _browser.MainFrame.Document;
        }

        public static bool NeedAuth(IDocument document) => document.GetElementById("loginForm") != null;

        public async Task SignIn(IDocument document, string redirectUrl)
        {
            document.GetElementByName("username").Attributes["value"] = Constants.UserName;
            document.GetElementByName("password").Attributes["value"] = Constants.Password;
            var submitBtn = _browser.MainFrame.Document.GetElementByClassName("L3NKy");
            if (submitBtn.Attributes.ContainsKey("disabled"))
            {
                submitBtn.Attributes.Remove("disabled");
            }
            var d = document.Frame.Html;
            submitBtn.Click();
            var d2 = document.Frame.Html;
            await _browser.Navigation.LoadUrl(redirectUrl);
            document = _browser.MainFrame.Document;
            var f = document.Frame.Html;
        }
    }
}
