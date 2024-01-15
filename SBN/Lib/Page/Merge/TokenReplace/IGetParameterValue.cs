using SBN.Lib.Action.Data;
using SBN.Lib.DB;
using SBN.Lib.Page.Request;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace SBN.Lib.Page.Merge.TokenReplace
{
    public interface IGetParameterValue
    {
        Task<string> Get(Guid sessionToken,
            string path,
            ValidatedRequest validatedRequest,
            List<ActionData> actionDataList,
            string parameter,
            IActions actions);
    }
}