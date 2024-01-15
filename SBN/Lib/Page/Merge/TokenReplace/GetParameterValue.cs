using SBN.Lib.Action.Data;
using SBN.Lib.Page.Request;
using System.Collections.Generic;
using System;
using System.Linq;
using SBN.Lib.DB;
using System.Threading.Tasks;

namespace SBN.Lib.Page.Merge.TokenReplace
{
    public class GetParameterValue : IGetParameterValue
    {
        private readonly IDataMergeParameterParser _dataMergeParameterParser;
        private readonly IBackfillData _backfillData;
        private readonly IValueFromData _valueFromData;

        public GetParameterValue(IDataMergeParameterParser dataMergeParameterParser,
            IBackfillData backfillData,
            IValueFromData valueFromData)
        {
            _dataMergeParameterParser = dataMergeParameterParser;
            _backfillData = backfillData;
            _valueFromData = valueFromData;
        }

        public async Task<string> Get(Guid sessionToken, 
            string path, 
            ValidatedRequest validatedRequest, 
            List<ActionData> actionDataList, 
            string parameter,
            IActions actions)
        {
            var dataMergeParameter = _dataMergeParameterParser.Parse(parameter);

            if (dataMergeParameter.ActionID == null) throw new Exception($"Cannot read parameter '{parameter}' without action ID being specified");

            var data = actionDataList.Where(x => x.ActionID == dataMergeParameter.ActionID);

            data = await _backfillData.Backfill(sessionToken, path, validatedRequest, actionDataList, dataMergeParameter, data, actions);

            if (data.Count() > 1) throw new Exception($"Cannot merge against action {dataMergeParameter.ActionID}, action ID repeated in data");

            var value = _valueFromData.Value(dataMergeParameter, data.Single());
            return value;
        }
    }
}
