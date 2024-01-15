using System;
using System.Net;
using SBN.Lib.Definitions;
using SBN.Models;

namespace SBN.Lib.Action.Outcome
{
    public class ApiActionOutcome
    {
        internal const string ActionOutcomeMarker = "__ActionOutcome";

        public Guid SessionToken { get; set; }

        public Response Body { get; set; }

        #region "Constructors"
        public ApiActionOutcome(HttpStatusCode statusCode)
        {
            Body = new Response { StatusCode = statusCode };
        }

        public ApiActionOutcome(HttpStatusCode statusCode, string outcomeMessage, OutcomeMessageType outcomeMessageType)
        {
            Body = new Response
            {
                StatusCode = statusCode,
                Message = outcomeMessage,
                MessageType = outcomeMessageType
            };
            if (outcomeMessage.StartsWith("User session has expired"))
            {
                Body.Url = $"/{Routes.NewSession}";
            }
        }

        public ApiActionOutcome(HttpStatusCode statusCode, 
            string outcomeMessage, 
            OutcomeMessageType outcomeMessageType, 
            string outcomeUrl)
        {
            Body = new Response
            {
                StatusCode = statusCode,
                Message = outcomeMessage,
                MessageType = outcomeMessageType,
                Url = outcomeUrl
            };
        }

        public ApiActionOutcome()
        {
        }
        #endregion
    }
}
