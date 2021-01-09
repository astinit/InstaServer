using DotNetBrowser.Dom;
using DotNetBrowser.Engine;
using InstaServer.BLL.Interfaces;
using System;
using System.Threading.Tasks;

namespace InstaServer.BLL.Helpers
{
    public class ParserWorker<T> where T : class
    {
        private IParserSettings _parserSettings;

        public IParser<T> Parser { get; set; }
        public IParserSettings ParserSettings
        {
            get => _parserSettings;
            set
            {
                _parserSettings = value;
                _htmlLoader = new HtmlLoader(value);
            }
        }

        private HtmlLoader _htmlLoader;
        private IDocument _htmlDocument;
        public ParserWorker(IParser<T> parser) => Parser = parser;
        public ParserWorker(IParser<T> parser, IParserSettings settings) : this(parser) => ParserSettings = settings;

        public async Task<IDocument> GetHtmlDocument()
        {
            _htmlDocument ??= await _htmlLoader.GetDocument();
            return _htmlDocument;
        }
        public async Task<T> DoWork() => await Parser.ParseAsync(await GetHtmlDocument(), _parserSettings);
    }
}
