using System.Collections.Generic;
using System.Threading.Tasks;

namespace SBN.Lib.Page.Render.Util
{
    public interface ISiteTextCache
    {
        Task<IDictionary<string, string>> Read(IEnumerable<string> parameter);
    }
}