using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SBN.Lib.DB;
using SBN.Lib.Page.Outcome;
using System;
using System.Threading.Tasks;

namespace SBN.Lib.Page.Call
{
    public class PageFromDB : IPageFromDB
    {
        private readonly IPages _pages;
        private readonly IPageDataParser _pageDataParser;
        private readonly IPageOutcome _pageOutcome;

        public PageFromDB(IPages pages,
            IPageDataParser pageDataParser,
            IPageOutcome pageOutcome)
        {
            _pages = pages;
            _pageDataParser = pageDataParser;
            _pageOutcome = pageOutcome;
        }

        public async Task<IActionResult> Get(Guid sessionToken, string pathAndQuery, string referrer, HttpContext httpContext)
        {
            var pageDataSet = await _pages.Get(sessionToken, pathAndQuery, referrer);

            var pageData = _pageDataParser.Parse(pageDataSet, httpContext, sessionToken);

            return await _pageOutcome.Build(pageData, httpContext);
        }
    }
}
