using SBN.Lib.Action.Outcome;
using SBN.Lib.DB.Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace SBN.Lib.Action.Data
{
    public class ActionDataMapper : IActionDataMapper
    {
        private const string ActionIDMarker = "$$ActionID";
        private const string ActionOutcomeMarker = "__ActionOutcome";

        private readonly IApiActionOutcomeDataPopulator _apiActionOutcomeBuilder;
        private readonly IActionDataBuilder _actionDataBuilder;

        public ActionDataMapper(IApiActionOutcomeDataPopulator apiActionOutcomeBuilder,
            IActionDataBuilder actionDataBuilder)
        {
            _apiActionOutcomeBuilder = apiActionOutcomeBuilder;
            _actionDataBuilder = actionDataBuilder;
        }

        public IEnumerable<ActionData> Map(DataTable[] data, Guid sessionToken)
        {
            DataRow header;
            int actionID = 0;
            string actionParameters = "";
            string sourceUrl = "";
            XElement definition = null;
            ApiActionOutcome outcome = null;
            XElement transform = null;
            DateTime? cacheExpiry = null;
            bool? cacheBySession = true;

            DataSet actionData = new();

            for (var i = 0; i < data.Length; i++)
            {
                switch (data[i].Columns[0].ColumnName)
                {
                    case ActionIDMarker:
                        if (actionID != 0)
                        {
                            yield return _actionDataBuilder.Build(sessionToken,
                                actionID,
                                actionParameters,
                                sourceUrl,
                                definition,
                                outcome,
                                transform,
                                cacheExpiry,
                                actionData);

                            actionID = 0;
                        }

                        header = data[i].Rows[0];
                        actionID = header.Field<int>(ActionIDMarker);
                        actionParameters = header.Field<string>("ActionParameters");
                        sourceUrl = header.Field<string>("SourceURL");
                        definition = header.XmlField("Definition");
                        actionData = new();
                        outcome = null;
                        transform = header.XmlField("Transform");
                        cacheExpiry = header.Field<DateTime?>("CacheExpiry");
                        cacheBySession = header.Field<bool?>("CacheBySession");
                        break;

                    case ActionOutcomeMarker:
                        outcome = data[i].Rows.Count == 0
                            ? null
                            : _apiActionOutcomeBuilder.Populate(data[i].Rows[0]);

                        yield return _actionDataBuilder.Build(sessionToken,
                            actionID,
                            actionParameters,
                            sourceUrl,
                            definition,
                            outcome,
                            transform,
                            cacheExpiry,
                            actionData);

                        actionID = 0;
                        break;

                    default:
                        actionData.Tables.Add(data[i].Copy());
                        break;
                }
            }

            if (actionID != 0)
            {
                yield return _actionDataBuilder.Build(sessionToken,
                    actionID,
                    actionParameters,
                    sourceUrl,
                    definition,
                    outcome,
                    transform,
                    cacheExpiry,
                    actionData);
            }
        }
    }
}