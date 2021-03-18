
using System.IO;

namespace InstaServer.BLL
{
    public static class Constants
    {
        public const string ImageKeyWord = "kPFhm";
        public const string CarouselImageKeyWord = "KL4Bh";
        public const string VideoKeyWord = "tWeCl";
        public const string CarouselKeyWord = "vi798";
        public const string MainTextContainerKeyWord = "PpGvg";

        public const string UserName = "uggamesstudio@gmail.com";
        public const string Password = "uggstudio2014";

        public const string InstagramLoginUrl = "https://www.instagram.com/accounts/login/";
        public const string BaseInstagramUrl = "https://www.instagram.com";
        public const string InstagramHost = "www.instagram.com";

        public static readonly string LogPath = Path.Combine(Directory.GetCurrentDirectory(), "log.txt");
    }
}
