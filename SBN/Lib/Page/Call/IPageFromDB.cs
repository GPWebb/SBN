using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace SBN.Lib.Page.Call
{
    public interface IPageFromDB
    {
        Task<IActionResult> Get(Guid sessionToken, string pathAndQuery, string referrer, HttpContext httpContext);
    }
}