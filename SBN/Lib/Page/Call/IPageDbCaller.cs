using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SBN.Lib.Page.Call
{
    public interface IPageDbCaller
    {
        Task<IActionResult> Call(Guid sessionToken, string pathAndQuery, string referrer, HttpContext httpContext);
    }
}