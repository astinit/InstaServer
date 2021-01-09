using InstaServer.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstaServer.BLL.Models
{
    public class InstaParserSettings : IParserSettings
    {
        public string Url { get; set; }
    }
}
