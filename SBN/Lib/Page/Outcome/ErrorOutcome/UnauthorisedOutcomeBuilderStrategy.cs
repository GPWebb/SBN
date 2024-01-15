using System.Net;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace SBN.Lib.Page.Outcome.ErrorOutcome
{
    public class UnauthorisedOutcomeBuilderStrategy : IErrorOutcomeBuilderStrategy
    {
        public IActionResult Build(HttpContext context, HttpStatusCode statusCode)
        {
            var url = HttpUtility.UrlEncode(context.Request.GetEncodedPathAndQuery());
            var msg = HttpUtility.UrlEncode(Definitions.Messages.PleaseLogInToView);
            var redirectUrl = $"{Definitions.Routes.LoginPage}?URL={url}&msg={msg}";

            return new RedirectResult(redirectUrl, permanent: false);
        }

        public bool Matches(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.Unauthorized;
        }
    }
}

