using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SBN.Lib.Action.Data;
using SBN.Lib.DB;
using SBN.Lib.Xml.XslTransform;

namespace SBN.Lib.Page.Outcome
{
    public class DynamicPagePartPopulator : IDynamicPagePartPopulator
    {
        private readonly IXmlTransformer _xmlTransformer;
        private readonly IXMLReader _xmlReader;

        public DynamicPagePartPopulator(IXmlTransformer xmlTransformer,
            IXMLReader xmlReader)
        {
            _xmlTransformer = xmlTransformer;
            _xmlReader = xmlReader;
        }

        public IEnumerable<PagePart> Populate(IEnumerable<PagePart> parts, List<ActionData> actionData)
        {
            foreach (var part in parts.Where(x => x.PartXML_ActionID != null))
            {
                ActionData data;
                try
                {
                    data = actionData.Single(x => x.ActionID == part.PartXML_ActionID);
                }
                catch (Exception ex)
                {
                    throw new Exception("Supplied action data could not be correctly matched against the requested data", ex);
                }

                XElement xml;
                try
                {
                    xml = _xmlReader.ReadFromDataTable(data.Data.Tables[0]);
                }
                catch (Exception ex)
                {
                    throw new Exception("Dynamic page part XML could not be parsed", ex);
                }

                var transform = data.Transform;

                if (transform != null)
                {
                    xml = _xmlTransformer.Transform(xml, transform);
                }

                part.PartXML = xml;
            }

            return parts;
        }
    }
}
