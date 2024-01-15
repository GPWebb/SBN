using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SBN.Lib.Page.Outcome.ErrorOutcome
{
    public class FallbackErrorOutcomeBuilderStrategy : IErrorOutcomeBuilderStrategy
    {
        //Fallback outcome builder.  Add more specific types as required
        public IActionResult Build(HttpContext context, HttpStatusCode statusCode)
        {
            return new StatusCodeResult((int)statusCode);
        }

        public bool Matches(HttpStatusCode statusCode)
        {
            return statusCode != HttpStatusCode.Unauthorized;
        }
    }
}
