using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SBN.Lib.Page.Outcome.ErrorOutcome
{
    public interface IErrorOutcomeBuilder
    {
        IActionResult Build(HttpStatusCode statusCode, HttpContext context);
    }
}