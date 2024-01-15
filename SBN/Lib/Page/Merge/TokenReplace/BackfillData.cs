using SBN.Lib.Action.Data;
using SBN.Lib.Page.Request;
using System.Collections.Generic;
using System;
using SBN.Lib.DB;
using System.Linq;
using System.Threading.Tasks;

namespace SBN.Lib.Page.Merge.TokenReplace
{
    public class BackfillData : IBackfillData
    {
        public async Task<IEnumerable<ActionData>> Backfill(
            Guid sessionToken, 
            string path, 
            ValidatedRequest validatedRequest, 
            List<ActionData> actionDataList, 
            DataMergeParameter dataMergeParameter, 
            IEnumerable<ActionData> data,
            IActions actions)
        {
            if (data == null || !data.Any())
            {
                //Last chance action loader, in case the actions directly specified in the page _themselves_ create a merge requirement
                actionDataList.Add(await actions.CallByID(sessionToken, (int)dataMergeParameter.ActionID, path, validatedRequest));
                data = actionDataList.Where(x => x.ActionID == dataMergeParameter.ActionID);
            }

            return data;
        }
    }
}
