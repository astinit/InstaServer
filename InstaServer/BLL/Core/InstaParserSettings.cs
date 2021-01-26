using InstaServer.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstaServer.BLL.Core
{
    public class InstaParserSettings : IParserSettings
    {
        public string Url { get; set; }
    }
}
