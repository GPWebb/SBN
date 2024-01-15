using SBN.Lib.Action.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace SBN.Lib.Page.Outcome
{
    public interface IActionDataSetParser
    {
        IEnumerable<ActionData> ParseActionData(
            DataSet pageDataSet,
            int actionDataStart,
            Guid sessionToken);
    }
}