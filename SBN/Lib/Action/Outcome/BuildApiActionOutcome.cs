using SBN.Lib.Action.Data;
using SBN.Models;
using System.Net;

namespace SBN.Lib.Action.Outcome
{
    public class BuildApiActionOutcome : IBuildApiActionOutcome
    {
        private readonly IBuildFromOutcomeResponse _buildFromOutcomeResponse;

        public BuildApiActionOutcome(IBuildFromOutcomeResponse buildFromOutcomeResponse)
        {
            _buildFromOutcomeResponse = buildFromOutcomeResponse;
        }

        public ApiActionOutcome Build(ActionData actionData)
        {
            var apiActionOutcome = new ApiActionOutcome();

            if (actionData.Data.Tables.Count > 0
                && actionData.Data.Tables[actionData.Data.Tables.Count - 1].Columns[0].ColumnName == ApiActionOutcome.ActionOutcomeMarker)
            {
                apiActionOutcome = _buildFromOutcomeResponse.Build(
                    actionData.Data.Tables[actionData.Data.Tables.Count - 1].Rows[0], 
                    apiActionOutcome);
            }
            else
            {
                apiActionOutcome = new ApiActionOutcome
                {
                    Body = new Response
                    {
                        StatusCode = actionData.StatusCode ?? HttpStatusCode.OK
                    }
                };
            }

            return apiActionOutcome;
        }
    }
}
