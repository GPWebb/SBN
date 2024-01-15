using SBN.Lib.Definitions;
using SBN.Models;
using System.Data;
using System.Xml.Linq;
using System;
using System.Net;

namespace SBN.Lib.Action.Outcome
{
    public class BuildFromOutcomeResponse : IBuildFromOutcomeResponse
    {
        public ApiActionOutcome Build(DataRow outcomeDataHeader, ApiActionOutcome apiActionOutcome)
        {
            bool success;
            
            success = Enum.TryParse(outcomeDataHeader.Field<int>("ResponseCode").ToString(), out HttpStatusCode httpStatusCode);
            if (!success) throw new ArgumentException($"'{outcomeDataHeader.Field<int>("ResponseCode")}' is not a valid response code");
            
            apiActionOutcome.Body = new Response
            {
                StatusCode = httpStatusCode
            };

            if (outcomeDataHeader.Field<string>("OutputParameters") != null)
            {
                apiActionOutcome.Body.OutputParameters = XElement.Parse(outcomeDataHeader.Field<string>("OutputParameters"));
            }
            apiActionOutcome.Body.APIRoute = outcomeDataHeader.Field<string>("APIRoute");

            if (!string.IsNullOrWhiteSpace(outcomeDataHeader.Field<string?>("Outcome_MessageType")))
            {
                success = Enum.TryParse(outcomeDataHeader.Field<string>("Outcome_MessageType"), out OutcomeMessageType outcomeMessageType);
                if (!success) throw new ArgumentException($"'{outcomeDataHeader.Field<string>("Outcome_MessageType")}' is not a valid message type");

                apiActionOutcome.Body.Message = outcomeDataHeader.Field<string>("Outcome_Message")?.ToString();
                apiActionOutcome.Body.MessageType = outcomeMessageType;
                apiActionOutcome.Body.Url = outcomeDataHeader.Field<string>("OutcomeURL")?.ToString();
            }

            return apiActionOutcome;
        }
    }
}
