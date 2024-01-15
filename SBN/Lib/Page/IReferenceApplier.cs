using System.Collections.Generic;
using System.Xml.Linq;

namespace SBN.Lib.Page
{
    public interface IReferenceApplier
    {
        XElement Apply(XElement pageTemplate, string pageReferences, IEnumerable<PagePart> pageParts);
    }
}