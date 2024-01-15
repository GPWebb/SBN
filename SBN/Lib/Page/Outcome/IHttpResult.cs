using Microsoft.AspNetCore.Mvc;

namespace SBN.Lib.Page.Outcome
{
    public interface IHttpResult
    {
        IActionResult Generate(string body);
    }
}