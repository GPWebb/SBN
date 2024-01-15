using System;
using System.Data.SqlClient;
using System.Net;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Xml.Linq;
using SBN.Lib.Action.Outcome;
using SBN.Lib.DB;
using SBN.Lib.Definitions;
using SBN.Lib.Page.Outcome;
using SBN.Lib.Sys;

namespace SBN.Lib.Action
{
    public class ApiActionDbCaller : IApiActionDbCaller
    {
        private readonly IActions _actions;
        private readonly IApiActionOutcomeDataPopulator _apiActionOutcomeBuilder;
        private readonly IActionDataParser _actionDataParser;

        public ApiActionDbCaller(IActions actions,
            IApiActionOutcomeDataPopulator apiActionOutcomeBuilder,
            IActionDataParser actionDataParser)
        {
            _actions = actions;
            _apiActionOutcomeBuilder = apiActionOutcomeBuilder;
            _actionDataParser = actionDataParser;
        }

        public async Task<ApiActionOutcome> Call(Guid sessionToken, 
            string pathAndQuery, 
            string verb, 
            string referrer, 
            bool decorateForJson, 
            XDocument requestBody)
        {
            try
            {
                var result = await _actions.Call(sessionToken, pathAndQuery, verb, referrer, requestBody);

                var actionData = _actionDataParser.ParseSingleAction(result.DataSet, result.Definition, result.Transform, sessionToken);

                return actionData.Outcome;
            }
            catch (SqlException ex)
            {
                var errorNumber = ex.Number.ToString();
                if (errorNumber.Length == 6 && errorNumber.StartsWith("580"))
                {
                    var statusCode = Enum.Parse<HttpStatusCode>(errorNumber.Substring(3));
                    return new ApiActionOutcome(statusCode, ex.Message, OutcomeMessageType.Error);
                }

                throw new Exception($"Error making {verb} call to {pathAndQuery}: {ex.Message}", ex);
            }
            catch (RestCallException rex)
            {
                return new ApiActionOutcome(rex.ResponseStatusCode, rex.Message, OutcomeMessageType.Error);
            }
        }
    }
}
