using DotNetBrowser.Dom;
using System.Threading.Tasks;

namespace InstaServer.BLL.Interfaces
{
    public interface IParser<T> where T: class
    {
        T Parse(IDocument document, IParserSettings settings);
        bool NeedAuth(IDocument document);
    }
}
