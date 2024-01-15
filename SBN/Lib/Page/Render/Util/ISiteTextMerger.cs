using System.Threading.Tasks;

namespace SBN.Lib.Page.Render.Util
{
    public interface ISiteTextMerger
    {
        Task<string> Merge(string pageTemplate);
    }
}