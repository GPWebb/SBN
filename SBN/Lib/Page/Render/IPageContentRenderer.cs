using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using SBN.Lib.Action.Data;
using SBN.Lib.Page.Request;

namespace SBN.Lib.Page.Render
{
    public interface IPageContentRenderer
    {
        Task<XElement> RenderPageContents(XElement pageTemplate,
            IEnumerable<string> permissions,
            IEnumerable<PagePart> pageParts,
            IEnumerable<ActionData> actionData,
            IEnumerable<PageDocument> pageDocuments,
            int maxPosition,
            string pageTitle,
            ValidatedRequest requestParams);
    }
}