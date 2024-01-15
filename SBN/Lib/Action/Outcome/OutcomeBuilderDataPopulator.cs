using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using SBN.Lib.Action.Data.Serialise;
using SBN.Lib.Action.Output;

namespace SBN.Lib.Action.Outcome
{
    public class OutcomeBuilderDataPopulator : IOutcomeBuilderDataPopulator
    {
        private readonly IOutputDefinitionParser _outputDefinitionParser;
        private IActionDataSerialiser _actionDataSerialiser;

        public OutcomeBuilderDataPopulator(IOutputDefinitionParser outputDefinitionParser,
            IActionDataSerialiser actionDataSerialiser)
        {
            _outputDefinitionParser = outputDefinitionParser;
            _actionDataSerialiser = actionDataSerialiser;
        }

        public void PopulateDataResponse(DataSet outcomeData,
            XElement outputDefinition,
            ApiActionOutcome apiActionOutcome,
            bool decorateForJson)
        {
            var tables = new List<DataTable>();

            for (var resultTable = 0; resultTable < outcomeData.Tables.Count; resultTable++)
            {
                if (outcomeData.Tables[resultTable].Columns[0].ColumnName != ApiActionOutcome.ActionOutcomeMarker)
                {
                    tables.Add(outcomeData.Tables[resultTable]);
                }
            }

            if (tables.Count == 1 && tables[0].Rows.Count == 0)
            {
                apiActionOutcome.Body.StatusCode = HttpStatusCode.NotFound;
            }
            else
            {
                var parsedDefinition = _outputDefinitionParser.Parse(outputDefinition);

                if (!parsedDefinition?.OutputSets.Any() ?? false) throw new Exception("No matching output definition found for table");

                apiActionOutcome.Body.Data = _actionDataSerialiser.Serialise(tables, parsedDefinition, decorateForJson);
            }
        }
    }
}