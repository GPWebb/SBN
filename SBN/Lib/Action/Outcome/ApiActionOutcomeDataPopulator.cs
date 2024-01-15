using Microsoft.AspNetCore.Http;
using SBN.Lib.Action.Data;
using SBN.Lib.Definitions;
using SBN.Lib.Sys;
using SBN.Models;
using System.Data;
using System.Net;

namespace SBN.Lib.Action.Outcome
{
    public class ApiActionOutcomeDataPopulator : IApiActionOutcomeDataPopulator
    {
        private readonly IOutcomeBuilderDataPopulator _outcomeBuilderDataPopulator;
        private readonly IOutcomeBuilderXmlPopulator _outcomeBuilderXmlPopulator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBuildFromOutcomeResponse _buildFromOutcomeResponse;
        private readonly IBuildApiActionOutcome _buildApiActionOutcome;
        private readonly ITransformActionData _transformActionData;

        public ApiActionOutcomeDataPopulator(IOutcomeBuilderDataPopulator outcomeBuilderDataPopulator,
            IOutcomeBuilderXmlPopulator outcomeBuilderXmlPopulator,
            IHttpContextAccessor httpContextAccessor,
            IBuildFromOutcomeResponse buildFromOutcomeResponse,
            IBuildApiActionOutcome buildApiActionOutcome,
            ITransformActionData transformActionData)
        {
            _outcomeBuilderDataPopulator = outcomeBuilderDataPopulator;
            _outcomeBuilderXmlPopulator = outcomeBuilderXmlPopulator;
            _httpContextAccessor = httpContextAccessor;
            _buildFromOutcomeResponse = buildFromOutcomeResponse;
            _buildApiActionOutcome = buildApiActionOutcome;
            _transformActionData = transformActionData;
        }

        public ApiActionOutcome Populate(DataRow outcomeDataHeader)
        {
            var apiActionOutcome = new ApiActionOutcome();

            if (outcomeDataHeader.Table.Columns[0].ColumnName == ApiActionOutcome.ActionOutcomeMarker)
            {
                apiActionOutcome = _buildFromOutcomeResponse.Build(outcomeDataHeader, apiActionOutcome);
            }
            else
            {
                apiActionOutcome.Body.StatusCode = HttpStatusCode.OK;
                apiActionOutcome.Body = new Response();
            }

            return apiActionOutcome;
        }

        public ApiActionOutcome Populate(ActionData actionData, bool decorateForJson, bool transform = true)
        {
            var apiActionOutcome = _buildApiActionOutcome.Build(actionData);

            //if (actionData.Data.Tables.Count == 0) throw new ArgumentException($"Action data for action {actionData.ActionID + actionData.ActionParameters} not populated");

            var populated = actionData.Data.Tables.Count == 0
                ? false
                : _outcomeBuilderXmlPopulator.PopulateXmlResponse(
                    actionData.Data.Tables[0],
                    populated: false,
                    apiActionOutcome.Body);

            if (populated && decorateForJson)
            {
                var request = _httpContextAccessor.HttpContext.Request;
                throw new RestCallException(request.Path.ToString(),
                    request.Method,
                    HttpStatusCode.BadRequest,
                    Messages.RequestedContentTypeUnavailable);
            }

            if (actionData?.Outcome?.Body?.Data != null) populated = true;

            if (!populated)
            {
                _outcomeBuilderDataPopulator.PopulateDataResponse(
                    actionData.Data,
                    actionData.Definition,
                    apiActionOutcome,
                    decorateForJson);

                //actionData.Outcome = apiActionOutcome;
            }

            if (transform && apiActionOutcome.Body.Data != null) _transformActionData.Transform(actionData, apiActionOutcome);

            return apiActionOutcome;
        }
    }
}