using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using SBN.Lib.Action.Data;
using SBN.Lib.Page.Request;

namespace SBN.Lib.Page.Render
{
    public interface IPartPreRenderer
    {
        Task<XElement> PreRenderPart(XElement pageTemplate, 
            IEnumerable<string> permissions, 
            IEnumerable<ActionData> actionData, 
            PagePart part,
            string pageTitle,
            ValidatedRequest requestParams);
    }
}