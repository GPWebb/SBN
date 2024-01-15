using System;
using System.Xml.Linq;
using SBN.Lib.Xml.XPath;

namespace SBN.Lib.Page.Merge
{
    public class XmlDataMerger : IXmlDataMerger
    {
        private IDataMergeParameterParser _dataMergeParameterParser;
        private readonly IXPathFacade _xpathFacade;

        public XmlDataMerger(IDataMergeParameterParser dataMergeParameterParser, IXPathFacade xPathFacade)
        {
            _dataMergeParameterParser = dataMergeParameterParser;
            _xpathFacade = xPathFacade;
        }

        public XElement Merge(XElement transformed, XElement data)
        {
            try
            {
                var output = transformed.ToString();

                var start = 0;
                while (output.IndexOf("[#", start) > 0)
                {
                    var position = output.IndexOf("[#", start);
                    var parameter = output.Substring(position + 2, output.IndexOf("]", position) - position - 2);

                    var dataMergeParameter = _dataMergeParameterParser.Parse(parameter);

                    if (dataMergeParameter.ActionID == null) //throw new Exception("Parameter cannot be specified by Action ID");
                    {
                        string replace = null;

                        try
                        {
                            var node = _xpathFacade.SelectElement(data, dataMergeParameter.XPathParameter);
                            replace = node?.Value ?? $"{dataMergeParameter.Parameter} not found";
                        }
                        catch (Exception e)
                        {
                            throw new Exception($"'{dataMergeParameter.Parameter}' not found in data", e);
                        }

                        output = output.Replace($"[#{dataMergeParameter.Parameter}]", replace);
                    }
                    start = position + 1;
                }

                return XElement.Parse(output);
            }
            catch (Exception e)
            {
                throw new Exception("Error merging data", e);
            }
        }
    }
}
