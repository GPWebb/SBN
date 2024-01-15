using SBN.Lib.Action.Data;
using SBN.Lib.Page.Request;
using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace SBN.Lib.Page.Outcome
{
    public class PageData
    {
        public string Title { get; set; }
        public DataRow ProfileData { get; set; }

        public XElement Template { get; set; }

        public string PageReferences { get; set; }

        public IEnumerable<PagePart> PageParts { get; set; }

        public ValidatedRequest ValidatedRequest { get; set; }

        public IEnumerable<string> PagePermissions { get; set; }

        public bool PageHasDocuments { get; set; }

        public IEnumerable<PageDocument> PageDocuments { get; set; }

        public IEnumerable<DocumentTypeResponse> DocumentTypeResponse { get; set; }

        public IEnumerable<ActionData> ActionData { get; set; }
        public DateTime? CacheExpiry { get; internal set; }
    }
}
