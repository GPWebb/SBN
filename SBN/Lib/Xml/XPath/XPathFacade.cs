using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Wmhelp.XPath2;

namespace SBN.Lib.Xml.XPath
{
    /// <summary>
    /// Evaluate XPath expressions
    /// </summary>
    /// <remarks>
    /// Only exists because MS' native XPath and XSLT is so appallingly dated and I needed to isolate the code for easier switching
    /// between libraries to provide better versions. 
    /// </remarks>
    public class XPathFacade : IXPathFacade
    {
        public IEnumerable<XElement> SelectElements(XElement document, string query)
        {
            try
            {
                return document.XPath2SelectElements(query);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error selecting elements with query '{query}': {ex.Message}", ex);
            }
        }

        public XElement SelectElement(XElement document, string query)
        {
            try
            {
                var result = SelectElements(document, query).ToList();

                if (result.Count > 1) throw new Exception($"Expected at most one node for '{query}', found {result.Count}");

                return result.SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error selecting elements with query '{query}': {ex.Message}", ex);
            }
        }

        public IEnumerable<object> Select(XElement document, string query)
        {
            try
            {
                return document.XPath2Select(query);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error selecting elements with query '{query}': {ex.Message}", ex);
            }
        }
    }
}
