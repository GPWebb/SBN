using SBN.Lib.Xml.XPath;
using System;
using System.Xml.Linq;

namespace SBN.Lib.Page.Merge.TokenReplace
{
    public class MergeFromDocument : IMergeFromDocument
    {
        private readonly IXPathFacade _xPathFacade;

        public MergeFromDocument(IXPathFacade xPathFacade)
        {
            _xPathFacade = xPathFacade;
        }

        public string Merge(DataMergeParameter dataMergeParameter, XElement document)
        {
            var xpath = dataMergeParameter.Parameter.Contains('/')
                ? dataMergeParameter.Parameter
                : $"//{dataMergeParameter.Parameter}";

            var value = _xPathFacade.SelectElement(document, xpath);

            if (value == null || value.Value == null)
                throw new Exception($"Merge parameter '{dataMergeParameter.Parameter}' not matched against document '{document}'");

            return value.Value;
        }

    }
}
