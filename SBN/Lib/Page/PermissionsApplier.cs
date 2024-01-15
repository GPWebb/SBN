using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SBN.Lib.Xml.XPath;

namespace SBN.Lib.Page
{
    public class PermissionsApplier : IPermissionsApplier
    {
        private readonly IXPathFacade _xpathFacade;

        public PermissionsApplier(IXPathFacade xpathFacade)
        {
            _xpathFacade = xpathFacade;
        }

        public XElement Apply(IEnumerable<string> permissions, XElement pageTemplate)
        {
            if (pageTemplate == null) return null;

            try
            {
                foreach (var element in _xpathFacade.SelectElements(pageTemplate, "//*[@Permission]"))
                {
                    if (!permissions.Any(p => p == element.Attribute("Permission").Value)) element.Remove();
                }

                return pageTemplate;
            }
            catch (Exception e)
            {
                throw new Exception($"Error applying permissions: {e.Message}", e);
            }
        }
    }
}