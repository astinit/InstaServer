using DotNetBrowser.Dom;
using Google.Protobuf.Collections;
using InstaServer.BLL.Helpers;
using InstaServer.BLL.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace InstaServer.BLL.Models
{
    public class InstaParser : IParser<PageData>
    {
        private Dictionary<MediaType, Func<IDocument, IParserSettings, Task<PageData>>> FunctionsParsingMediaType { get; }
        public InstaParser()
        => FunctionsParsingMediaType = new Dictionary<MediaType, Func<IDocument, IParserSettings, Task<PageData>>>()
            {
                { MediaType.Image, GetImagePageData },
                { MediaType.Carousel, GetCarouselPageData },
                { MediaType.Video, GetVideoPageData },
                { MediaType.Reel, GetReelPageData  },
                { MediaType.Igtv, GetIGTVPageData  },
                { MediaType.Undefined, GetEmptyPageData  },
            };

        public async Task<PageData> ParseAsync(IDocument document, IParserSettings settings)
        {
            var instaParserHelper = new InstaParserHelper<PageData>();
            //if (HtmlLoader.NeedAuth(document))
            //{
            //    var uri = new Uri(settings.Url);
            //    settings.Url = Constants.BaseUrl + HttpUtility.ParseQueryString(uri.Query).Get("next");
            //    await HtmlLoader.SignIn(document, settings.Url);
            //}
            var currentLinkType = instaParserHelper.GetLinkType(settings.Url);
            var mediaType = instaParserHelper.FunctionsMediaTypeDefinition[currentLinkType].Invoke(document);
            try
            {
                var pageData = await FunctionsParsingMediaType[mediaType].Invoke(document, settings);
                if(mediaType != MediaType.Undefined)
                {
                    var decodedHtml = HttpUtility.HtmlDecode(GetMainText(document)).Replace("<br>", "\n");
                    pageData.PostText = Regex.Replace(decodedHtml, "<[^>]+>", string.Empty);
                    var hashtags = Regex.Matches(@"^#\D*", pageData.PostText).Select(mc => mc.Value).ToArray();
                }
                return pageData;
            }
            catch (Exception ex)
            {
                //TODO: log error
                var loggerFactory = LoggerFactory.Create(builder => builder.AddEventLog());
                var logger = loggerFactory.CreateLogger<InstaParser>();
                logger.LogError(JsonConvert.SerializeObject(ex));
                logger.LogInformation(document.Frame.Html);
                return null;
            }
        }

        private async Task<PageData> GetImagePageData(IDocument document, IParserSettings settings)
        {
            var pageData = new PageData() { PageMediaType = MediaType.Image };
            pageData.PageItems.Add(new PageItem() { ItemMediaType = MediaType.Image });
            pageData.PageItems[0].Link = GetImageElement(document, Constants.ImageKeyWord).Attributes["src"];
            GetImageTags(document, pageData.PageItems[0].Tags);
            pageData.PageItems[0].File = await GetEncodedFiles(pageData.PageItems[0].Link);
            return pageData;
        }

        private async Task<PageData> GetVideoPageData(IDocument document, IParserSettings settings)
        {
            var pageData = new PageData() { PageMediaType = MediaType.Video };
            pageData.PageItems.Add(new PageItem() { ItemMediaType = MediaType.Video });
            pageData.PageItems[0].Link = GetVideoElement(document).Attributes["src"];
            GetVideoTags(document, document, pageData.PageItems[0].Tags);
            pageData.PageItems[0].File = await GetEncodedFiles(pageData.PageItems[0].Link);
            return pageData;
        }
        private async Task<PageData> GetIGTVPageData(IDocument document, IParserSettings settings)
        {
            var pageData = new PageData() { PageMediaType = MediaType.Igtv };
            pageData.PageItems.Add(new PageItem() { ItemMediaType = MediaType.Igtv });
            pageData.PageItems[0].Link = GetVideoElement(document).Attributes["src"];
            GetVideoTags(document, document, pageData.PageItems[0].Tags);
            pageData.PageItems[0].File = await GetEncodedFiles(pageData.PageItems[0].Link);
            return pageData;
        }
        private async Task<PageData> GetReelPageData(IDocument document, IParserSettings settings)
        {
            var pageData = new PageData() { PageMediaType = MediaType.Reel };
            pageData.PageItems.Add(new PageItem() { ItemMediaType = MediaType.Reel });
            pageData.PageItems[0].Link = GetVideoElement(document).Attributes["src"];
            GetVideoTags(document, document, pageData.PageItems[0].Tags);
            pageData.PageItems[0].File = await GetEncodedFiles(pageData.PageItems[0].Link);
            return pageData;
        }
        private async Task<PageData> GetCarouselPageData(IDocument document, IParserSettings settings)
        {
            var pageData = new PageData() { PageMediaType = MediaType.Carousel };
            IElement nextBtn;
            List<IElement> carouselItems = new List<IElement>();
            var ul = document.GetElementByClassName("vi798");
            var i = 1;
            while ((nextBtn = document.GetElementByClassName("coreSpriteRightChevron")) != null)
            {
                var currentCarouselItems = ul.GetElementsByTagName("li").ToArray();
                var f = ul.InnerHtml;
                if (i == 1)
                {
                    carouselItems.Add(currentCarouselItems[i]);
                    i++;
                }
                var lastElement = currentCarouselItems.Last();
                if (lastElement.InnerHtml != carouselItems.Last().InnerHtml)
                {
                    carouselItems.Add(lastElement);
                }
                nextBtn.Click();
            }
            i = 0;
            foreach (var carouselItem in carouselItems)
            {
                var element = GetImageElement(carouselItem, Constants.CarouselImageKeyWord);
                pageData.PageItems.Add(new PageItem());
                if(element != null)
                {
                    pageData.PageItems[i].Link = element.Attributes["src"];
                    pageData.PageItems[i].ItemMediaType = MediaType.Image;
                    GetImageTags(carouselItem, pageData.PageItems[i].Tags);
                }
                else
                {
                    element = GetVideoElement(carouselItem);
                    pageData.PageItems[i].Link = element.Attributes["src"];
                    pageData.PageItems[i].ItemMediaType = MediaType.Video;
                    GetVideoTags(document, carouselItem, pageData.PageItems[i].Tags);
                }
                pageData.PageItems[i].File = await GetEncodedFiles(pageData.PageItems[i].Link);
                i++;
            }
            return pageData;
        }
        private async Task<PageData> GetEmptyPageData(IDocument document, IParserSettings settings)
        {
            var pageData = new PageData() { PageMediaType = MediaType.Undefined };
            return pageData;
        }

        private IElement GetImageElement(INode node, string containerKeyWord)
        {
            var imgContainer = node.GetElementByClassName(containerKeyWord);
            if (imgContainer != null)
            {
                return imgContainer.GetElementByTagName("img");
            }
            return null;
        }

        private IElement GetVideoElement(INode node)
        {
            var videos = node.GetElementsByTagName("video");
            if (videos != null && videos.Any())
            {
                return videos.FirstOrDefault(item => item.Attributes["class"].Contains("tWeCl"));
            }
            return null;
        }

        private string GetMainText(IDocument document)
        {
            string mainText = null;
            var mainTextContainer = document.GetElementByClassName(Constants.MainTextContainerKeyWord);
            mainText = mainTextContainer.GetElementsByTagName("span").FirstOrDefault(span => string.IsNullOrEmpty(span.Attributes["class"])).InnerHtml;
            return mainText;
        }

        private async Task<string> GetEncodedFiles(string link)
        {
            using var httpClient = new HttpClient();
            var file = await httpClient.GetByteArrayAsync(link);
            return Convert.ToBase64String(file);
        }

        private void GetImageTags(INode node, IList<string> tags)
        {
            foreach (var tag in node.GetElementsByClassName("eg3Fv").Select(e => e.InnerText))
            {
                tags.Add(tag);
            }
        }

        private void GetVideoTags(IDocument document, INode node, IList<string> tags)
        {
            var tagBtn = node.GetElementByClassName("G_hoz");
            if(tagBtn != null)
            {
                tagBtn.Click();
                var dialogWindow = document.GetElementByClassName("_1XyCr");
                foreach (var tag in dialogWindow.GetElementsByClassName("qyrsm").Select(e => e.InnerText))
                {
                    tags.Add(tag);
                }
            }
        }
    }
}
