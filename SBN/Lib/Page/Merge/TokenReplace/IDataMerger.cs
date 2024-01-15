using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SBN.Lib.Action.Data;
using SBN.Lib.Page.Request;

namespace SBN.Lib.Page.Merge.TokenReplace
{
    public interface IDataMerger
    {
        Task<string> MergeData(string pageTemplate, IEnumerable<ActionData> actionData, Guid sessionToken, string path, ValidatedRequest validatedRequest);
    }
}