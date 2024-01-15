using SBN.Lib.Action.Data;
using SBN.Lib.DB;
using SBN.Lib.Page.Request;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace SBN.Lib.Page.Merge.TokenReplace
{
    public interface IBackfillData
    {
        Task<IEnumerable<ActionData>> Backfill(
           Guid sessionToken,
           string path,
           ValidatedRequest validatedRequest,
           List<ActionData> actionDataList,
           DataMergeParameter dataMergeParameter,
           IEnumerable<ActionData> data,
           IActions actions);
    }
}