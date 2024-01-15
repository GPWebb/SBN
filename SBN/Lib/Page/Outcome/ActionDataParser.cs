using SBN.Lib.Action.Data;
using SBN.Lib.Action.Outcome;
using SBN.Lib.Sys;
using System;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace SBN.Lib.Page.Outcome
{
    public class ActionDataParser : IActionDataParser
    {
        private readonly IActionDataMapper _actionDataMapper;
        private readonly IApiActionOutcomeDataPopulator _apiActionOutcomeDataPopulator;
        private readonly IXmlTryReader _xmlTryReader;

        public ActionDataParser(IActionDataMapper actionDataMapper,
            IApiActionOutcomeDataPopulator apiActionOutcomeDataPopulator,
            IXmlTryReader xmlTryReader)
        {
            _actionDataMapper = actionDataMapper;
            _apiActionOutcomeDataPopulator = apiActionOutcomeDataPopulator;
            _xmlTryReader = xmlTryReader;
        }

        public ActionData ParseSingleAction(DataSet actionDataSet, string definition, string transform, Guid sessionToken)
        {
            Exception exception;
            XElement definitionXml = null;
            XElement transformXml = null;

            if (!string.IsNullOrWhiteSpace(definition) && !_xmlTryReader.TryRead(definition, out definitionXml, out exception))
            {
                throw new Exception($"Invalid action data definition supplied: {exception.Message}", exception);
            }

            if (!string.IsNullOrWhiteSpace(transform) && !_xmlTryReader.TryRead(transform, out transformXml, out exception))
            {
                throw new Exception($"Invalid action data transformation supplied: {exception.Message}", exception);
            }

            var actionData = _actionDataMapper
                .Map(actionDataSet.Tables.Cast<DataTable>().ToArray(), sessionToken)
                .Single();

            _apiActionOutcomeDataPopulator.Populate(actionData, false);

            actionData.Definition = definitionXml;
            actionData.Transform = transformXml;

            return actionData;
        }
    }
}
