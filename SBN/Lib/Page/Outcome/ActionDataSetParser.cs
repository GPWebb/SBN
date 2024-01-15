using SBN.Lib.Action.Data;
using SBN.Lib.Action.Outcome;
using SBN.Lib.Sys;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SBN.Lib.Page.Outcome
{
    public class ActionDataSetParser : IActionDataSetParser
    {
        private readonly IActionDataMapper _actionDataMapper;
        private readonly IApiActionOutcomeDataPopulator _apiActionOutcomeDataPopulator;

        public ActionDataSetParser(IActionDataMapper actionDataMapper,
            IApiActionOutcomeDataPopulator apiActionOutcomeDataPopulator)
        {
            _actionDataMapper = actionDataMapper;
            _apiActionOutcomeDataPopulator = apiActionOutcomeDataPopulator;
        }

        public IEnumerable<ActionData> ParseActionData(
            DataSet pageDataSet,
            int actionDataStart,
            Guid sessionToken)
        {
            if (pageDataSet.Tables.Count > actionDataStart)
            {
                var data = pageDataSet.Tables
                    .Cast<DataTable>()
                    .Skip(actionDataStart)
                    .ToArray();

                var actionData = _actionDataMapper
                    .Map(data, sessionToken)
                    .ToList();

                foreach (var action in actionData.Where(x => x.Outcome?.Body?.StatusCode.IsSuccessStatusCode() ?? false))
                {
                    _apiActionOutcomeDataPopulator.Populate(action, false);
                }

                return actionData;
            }

            return null;
        }
    }
}
