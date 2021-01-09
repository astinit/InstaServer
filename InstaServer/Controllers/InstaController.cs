using InstaServer.BLL.Helpers;
using InstaServer.BLL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstaServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstaController : ControllerBase
    {
        public async Task<PageData> GetPageData(string link)
        {
            bool isLink = Uri.IsWellFormedUriString(link, UriKind.Absolute);
            if (!isLink)
            {
                //TODO Throw exception
                return null;
            }
            var parserWorker = new ParserWorker<PageData>(new InstaParser(), new InstaParserSettings() { Url = link });
            return await parserWorker.DoWork();
        }
    }
}
