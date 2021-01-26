using DotNetBrowser.Dom;
using System.Threading.Tasks;

namespace InstaServer.BLL.Interfaces
{
    public interface IParser<T> where T: class
    {
        Task<T> ParseAsync(IDocument document, IParserSettings settings);
        bool NeedAuth(IDocument document);
    }
}
