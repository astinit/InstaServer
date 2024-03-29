﻿using InstaServer.BLL.Helpers;
using InstaServer.BLL.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using InstaServer.BLL;
using InstaServer.Logging;

namespace InstaServer.WebAPIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstaController : ControllerBase
    {
        private readonly ILogger<InstaController> _logger;
        public InstaController(ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile(Constants.LogPath);
            _logger = loggerFactory.CreateLogger<InstaController>();
        }
        public async Task<PageData> GetPageData(string link)
        {
            var pageData = new PageData();
            if (!string.IsNullOrWhiteSpace(link))
            {
                bool isLink = Uri.IsWellFormedUriString(link, UriKind.Absolute);
                if (!isLink)
                {
                    pageData.Error = Error.UrlException;
                }
                else
                {
                    var parserWorker = new ParserWorker<PageData>(new InstaParser(), new InstaParserSettings() { Url = link });
                    pageData = await parserWorker.DoWork();
                }
            }
            else
            {
                pageData.Error = Error.UrlException;
            }
            return pageData;
        }

        [HttpPut]
        public async Task<FileLoadResponse> GetFiles([FromBody] IEnumerable<string> links)
        {
            var fileResponse = new FileLoadResponse();
            if (links != null && links.Any())
            {
                bool isLinks = !links.Any(l => !Uri.IsWellFormedUriString(l, UriKind.Absolute));
                if (!isLinks)
                {
                    fileResponse.Error = Error.UrlException;
                }
                else
                {
                    fileResponse = await FileLoader.GetFileLoadResponse(links);
                }
            }
            else
            {
                fileResponse.Error = Error.UrlException;
            }
            return fileResponse;
        }
    }
}
