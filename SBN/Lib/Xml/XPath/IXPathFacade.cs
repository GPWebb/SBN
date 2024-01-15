using System.Collections.Generic;
using System.Xml.Linq;

namespace SBN.Lib.Xml.XPath
{
    public interface IXPathFacade
    {
        IEnumerable<XElement> SelectElements(XElement document, string query);
        XElement SelectElement(XElement document, string query);
        IEnumerable<object> Select(XElement document, string query);
    }
}