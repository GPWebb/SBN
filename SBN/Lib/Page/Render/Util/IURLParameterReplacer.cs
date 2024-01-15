using System.Xml.Linq;
using SBN.Lib.Page.Request;

namespace SBN.Lib.Page.Render.Util
{
    public interface IURLParameterReplacer
    {
        string Replace(string pageTemplate, ValidatedRequest validatedRequest);
    }
}