using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SBN.Lib.Request;

namespace SBN.Lib.Page.Call
{
    public class PageCaller : IPageCaller
    {
        private readonly IPageDbCaller _pageDbCaller;
        private readonly IRequestReader _requestReader;
        private readonly IEstablishSessionToken _establishSessionToken;

        public PageCaller(IPageDbCaller pageDbCaller,
            IRequestReader requestReader,
            IEstablishSessionToken establishSessionToken)
        {
            _pageDbCaller = pageDbCaller;
            _requestReader = requestReader;
            _establishSessionToken = establishSessionToken;
        }

        public async Task<IActionResult> Call(HttpRequest request)
        {
            Guid sessionToken = await _establishSessionToken.Establish();

            var pathAndQuery = _requestReader.PathAndQuery(request);
            var referrer = _requestReader.Referrer(request);
            var context = request.HttpContext;

            return await _pageDbCaller.Call(sessionToken, pathAndQuery, referrer, context);
        }
    }
}