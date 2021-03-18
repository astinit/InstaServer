using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace InstaServer.WebAPIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        public string GetLogs() => System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "log.txt"));
    }
}
