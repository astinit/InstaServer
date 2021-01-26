using DotNetBrowser.Dom;
using DotNetBrowser.Engine;
using InstaServer.BLL.Helpers;
using InstaServer.BLL.Interfaces;
using System;
using System.Threading.Tasks;
using System.Web;

namespace InstaServer.BLL.Core
{
    public class ParserWorker<T> where T : class
    {
        public IParser<T> Parser { get; private set; }
        public IParserSettings ParserSettings { get; private set; }

        private readonly BrowserManager _browserManager;
        private IDocument _htmlDocument;
        public ParserWorker(IParser<T> parser, IParserSettings settings)
        {
            Parser = parser;
            ParserSettings = settings;
            _browserManager = new BrowserManager(settings);
        }

        public async Task<IDocument> GetHtmlDocument()
        {
            _htmlDocument ??= await _browserManager.GetDocument();
            return _htmlDocument;
        }
        public async Task<T> DoWork()
        {
            var document = await GetHtmlDocument();
            if (Parser.NeedAuth(document))
            {
                //TODO: Send respone about login needed
                //var uri = new Uri(_parserSettings.Url);
                //_parserSettings.Url = Constants.BaseUrl + HttpUtility.ParseQueryString(uri.Query).Get("next");
            }
            return await Parser.ParseAsync(document, ParserSettings);
        }
    }
}
