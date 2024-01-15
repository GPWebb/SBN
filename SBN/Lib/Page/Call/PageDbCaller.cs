using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SBN.Lib.DB;
using SBN.Lib.Page.Outcome.ErrorOutcome;
using SBN.Lib.Page.Render;
using SBN.Lib.Sys;
using System;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SBN.Lib.Page.Call
{
    public class PageDbCaller : IPageDbCaller
    {
        private readonly IAnalytics _analytics;
        private readonly IErrorOutcomeBuilder _errorOutcomeBuilder;
        private readonly IPageCache _pageCache;
        private readonly IPageFromDB _pageFromDB;
        private readonly IPageFromCache _pageFromCache;
        private readonly IXmlTryReader _xmlTryReader;

        public PageDbCaller(IAnalytics analytics,
            IErrorOutcomeBuilder errorOutcomeBuilder,
            IPageCache pageCache,
            IPageFromDB pageFromDB,
            IPageFromCache pageFromCache,
            IXmlTryReader xmlTryReader)
        {
            _analytics = analytics;
            _errorOutcomeBuilder = errorOutcomeBuilder;
            _pageCache = pageCache;
            _pageFromDB = pageFromDB;
            _pageFromCache = pageFromCache;
            _xmlTryReader = xmlTryReader; 
        }

        public async Task<IActionResult> Call(Guid sessionToken, string pathAndQuery, string referrer, HttpContext httpContext)
        {
            try
            {
                await _analytics.RecordHit(sessionToken,
                    $"{httpContext.Request.Scheme}://{httpContext.Request.Host.Value}",
                    pathAndQuery,
                    referrer,
                    DateTime.UtcNow);

                var cachedPage = _pageCache.Get(pathAndQuery);
                XElement pageXml = null;

                var validPageInCache = string.IsNullOrWhiteSpace(cachedPage) 
                    ? false
                    : _xmlTryReader.TryRead(cachedPage, out pageXml, out var exception);

                if (validPageInCache)
                {
                    return await _pageFromCache.Get(sessionToken, pageXml);
                }
                else
                {
                    return await _pageFromDB.Get(sessionToken, pathAndQuery, referrer, httpContext);
                }
            }
            catch (SqlException ex)
            {
                var errorNumber = ex.Number.ToString();
                if (errorNumber.Length == 6 && errorNumber.StartsWith("580"))
                {
                    var statusCode = Enum.Parse<HttpStatusCode>(errorNumber.Substring(3));
                    return _errorOutcomeBuilder.Build(statusCode, httpContext);
                }

                throw new Exception("Unable to load page from database", ex);
            }
        }
    }
}
