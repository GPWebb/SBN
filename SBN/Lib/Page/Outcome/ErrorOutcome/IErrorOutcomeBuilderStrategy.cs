using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SBN.Lib.Page.Outcome.ErrorOutcome
{
    public interface IErrorOutcomeBuilderStrategy
    {
        bool Matches(HttpStatusCode statusCode);
        IActionResult Build(HttpContext context, HttpStatusCode statusCode);
    }
}