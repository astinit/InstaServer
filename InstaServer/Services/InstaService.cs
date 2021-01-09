using Grpc.Core;
using InstaServer.BLL;
using InstaServer.BLL.Helpers;
using InstaServer.BLL.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DotNetBrowser.Browser;
using DotNetBrowser.Engine;

namespace InstaServer.Services
{
    public class InstaService : Insta.InstaBase
    {
        private readonly ILogger<InstaService> _logger;
        public InstaService(ILogger<InstaService> logger) => _logger = logger;
        public override async Task<PageData> GetPageData(PageLink request, ServerCallContext context)
        {
            bool isLink = Uri.IsWellFormedUriString(request.Link, UriKind.Absolute);
            if (!isLink)
            {
                //TODO Throw exception
                return null;
            }
            var parserWorker = new ParserWorker<PageData>(new InstaParser(), new InstaParserSettings() { Url = request.Link });
            return await parserWorker.DoWork();
        }
    }
}
