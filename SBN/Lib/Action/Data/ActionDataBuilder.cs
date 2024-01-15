using SBN.Lib.Action.Outcome;
using System.Data;
using System.Xml.Linq;
using System;

namespace SBN.Lib.Action.Data
{
    public class ActionDataBuilder : IActionDataBuilder
    {
        private readonly IActionDataLoader _actionDataLoader;

        public ActionDataBuilder(IActionDataLoader actionDataLoader)
        {
            _actionDataLoader = actionDataLoader;
        }

        public ActionData Build(Guid sessionToken,
            int actionID,
            string actionParameters,
            string sourceUrl,
            XElement definition,
            ApiActionOutcome outcome,
            XElement transform,
            DateTime? cacheExpiry,
            DataSet actionData)
        {
            var returnData = new ActionData
            {
                ActionID = actionID,
                ActionParameters = actionParameters,
                SourceURL = sourceUrl,
                Definition = definition,
                Data = actionData,
                Outcome = outcome,
                Transform = transform,
                CacheExpiry = cacheExpiry
            };

            if (returnData.Data.Tables.Count == 0)
            {
                _actionDataLoader.Load(returnData, sessionToken, sourceUrl);
            }

            return returnData;
        }
    }
}
