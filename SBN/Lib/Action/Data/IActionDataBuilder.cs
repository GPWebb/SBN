using SBN.Lib.Action.Outcome;
using System.Data;
using System.Xml.Linq;
using System;
using SBN.Lib.Page.Request;

namespace SBN.Lib.Action.Data
{
    public interface IActionDataBuilder
    {
        ActionData Build(Guid sessionToken,
            int actionID,
            string actionParameters,
            string sourceUrl,
            XElement definition,
            ApiActionOutcome outcome,
            XElement transform,
            DateTime? cacheExpiry,
            DataSet actionData);
    }
}