using InstaServer.BLL.Helpers;
using InstaServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstaServer.WebAPIControllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        [HttpPost]
        public async Task<int> PostSessionId([FromBody] string sessionId)
        {
            var result = await BrowserManager.SetCookie("sessionid", sessionId);
            return result.Exception != null ? StatusCodes.Status500InternalServerError : StatusCodes.Status200OK;
        }

        [HttpPost]
        public async Task<int> PostCookies([FromBody] BrowserCookie cookie)
        {
            var result = await BrowserManager.SetCookies(cookie);
            return result.Exception != null ? StatusCodes.Status500InternalServerError : StatusCodes.Status200OK;
        }
    }
}