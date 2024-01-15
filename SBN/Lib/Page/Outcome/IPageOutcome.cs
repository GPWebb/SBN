using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SBN.Lib.Page.Outcome
{
    public interface IPageOutcome
    {
        Task<IActionResult> Build(PageData pageData, HttpContext context);
    }
}