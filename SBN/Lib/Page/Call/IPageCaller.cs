using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SBN.Lib.Page.Call
{
    public interface IPageCaller
    {
        Task<IActionResult> Call(HttpRequest request);
    }
}