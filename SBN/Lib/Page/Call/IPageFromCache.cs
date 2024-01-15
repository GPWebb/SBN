using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SBN.Lib.Page.Call
{
    public interface IPageFromCache
    {
        Task<IActionResult> Get(Guid sessionToken, XElement cachedPageXml);
    }
}