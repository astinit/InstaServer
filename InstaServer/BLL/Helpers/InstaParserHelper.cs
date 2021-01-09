using DotNetBrowser.Dom;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InstaServer.BLL.Helpers
{
    public class InstaParserHelper<T> where T : class
    {
        public Dictionary<string, LinkType> LinkTypes { get; }
        public Dictionary<string, MediaType> MediTypeKeyWords { get; }
        public Dictionary<LinkType, Func<IDocument, MediaType>> FunctionsMediaTypeDefinition { get; }
        public InstaParserHelper()
        {
            LinkTypes = new Dictionary<string, LinkType>()
            {
                { "p", LinkType.P },
                { "reel", LinkType.R },
                { "tv", LinkType.T },
            };
            MediTypeKeyWords = new Dictionary<string, MediaType>()
            {
                { Constants.ImageKeyWord, MediaType.Image },
                { Constants.CarouselKeyWord, MediaType.Carousel  },
                { Constants.VideoKeyWord, MediaType.Video  },
                { "", MediaType.Undefined  },
                //{ , MediaType.Igtv  },
                //{ , MediaType.Reel  },
            };
            FunctionsMediaTypeDefinition = new Dictionary<LinkType, Func<IDocument, MediaType>>()
            {
                { LinkType.P, DefinePLinkMediaType },
                { LinkType.R, DefineReelLinkMediaType  },
                { LinkType.T, DefineTVLinkMediaType  },
                { LinkType.U, SetUndefinedMediaType },
            };
        }

        public LinkType GetLinkType(string link)
        {
            var uri = new Uri(link);
            if(uri.Segments.Count() > 1)
            {
                var currentLinkType = uri.Segments[1].Replace("/", "");
                if (!string.IsNullOrEmpty(currentLinkType) && LinkTypes.ContainsKey(currentLinkType))
                {
                    return LinkTypes[currentLinkType];
                }
            }
            return LinkType.U;
        }

        private MediaType DefinePLinkMediaType(IDocument document)
        {
            try
            {
                var keyWord = document.GetElementByClassName(Constants.CarouselKeyWord) != null ? Constants.CarouselKeyWord : null;
                keyWord ??= document.GetElementByClassName(Constants.VideoKeyWord) != null ? Constants.VideoKeyWord : null;
                keyWord ??= document.GetElementByClassName(Constants.ImageKeyWord) != null ? Constants.ImageKeyWord : null;
                keyWord ??= "";
                return MediTypeKeyWords[keyWord];
            }
            catch(Exception ex)
            {
                return MediaType.Undefined;
            }
        }

        private static MediaType DefineReelLinkMediaType(IDocument document) => MediaType.Reel;

        private static MediaType DefineTVLinkMediaType(IDocument document) => MediaType.Igtv;
        private static MediaType SetUndefinedMediaType(IDocument document) => MediaType.Undefined;

    }
}
