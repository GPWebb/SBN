using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using SBN.Lib.Action.Data;
using SBN.Lib.Page.Request;

namespace SBN.Lib.Page.Render
{
    public interface IPageRenderer
    {
        Task<string> Render(XElement pageTemplate,
            string pageTitle,
            string pageReferences,
            IEnumerable<string> pagePermissions,
            IEnumerable<PagePart> pageParts,
            Guid sessionToken,
            string path,
            string pathAndQuery,
            ValidatedRequest validatedRequest,
            IEnumerable<ActionData> actionData,
            IEnumerable<PageDocument> pageDocuments,
            DateTime? cacheExpiry);
    }
}