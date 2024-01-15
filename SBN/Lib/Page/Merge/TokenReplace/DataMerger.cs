using SBN.Lib.Action.Data;
using SBN.Lib.DB;
using SBN.Lib.Page.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SBN.Lib.Page.Merge.TokenReplace
{
    public class DataMerger : IDataMerger
    {
        private readonly IActions _actions;
        private readonly IParameterReader _parameterReader;
        private readonly IGetParameterValue _getParameterValue;

        public DataMerger(IActions actions,
            IParameterReader parameterReader,
            IGetParameterValue getParameterValue)
        {
            _actions = actions;
            _parameterReader = parameterReader;
            _getParameterValue = getParameterValue;
        }

        public async Task<string> MergeData(string pageTemplate,
            IEnumerable<ActionData> actionData,
            Guid sessionToken,
            string path,
            ValidatedRequest validatedRequest)
        {
            var output = pageTemplate;
            var actionDataList = actionData.ToList();

            if (!output.Contains("[#")) return pageTemplate;

            while (output.Contains("[#"))
            {
                var parameter = _parameterReader.Read(output);

                var value = await _getParameterValue.Get(sessionToken, path, validatedRequest, actionDataList, parameter, _actions);

                output = output.Replace($"[#{parameter}]", value);
            }

            return output;
        }
    }
}
