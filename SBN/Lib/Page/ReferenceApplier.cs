using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SBN.Lib.Page
{
    public class ReferenceApplier : IReferenceApplier
    {
        public XElement Apply(XElement pageTemplate, string pageReferences, IEnumerable<PagePart> pageParts)
        {
            try
            {
                var references = pageParts
                    .Where(x => x.Reference != null)
                    .Select(x => x.Reference)
                    .Distinct()
                    .ToList();

                if(!string.IsNullOrWhiteSpace(pageReferences)) references.Add(pageReferences);

                foreach (var reference in references)
                {
                    var tags = reference.Split('|');
                    foreach (var tag in tags)
                    {
                        var element = XElement.Parse(tag);
                        var body = pageTemplate
                            .Element("body")
                            ?.Elements("script");

                        var target = (body.Count() == 0)
                            ? pageTemplate.Element("body")
                            : body.Last();

                        target.AddAfterSelf(element);
                    }
                }
                return pageTemplate;
            }
            catch(Exception ex)
            {
                throw new Exception($"Error applying reference: {ex.Message}", ex);
            }
        }
    }
}
