using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SBN.Lib.Page.Call;
using SBN.Lib.Page.Outcome.ErrorOutcome;
using SBN.Lib.Sys;

namespace SBN.Controllers
{
    public class PageController : Controller
    {
        private readonly IPageCaller _pageCaller;
        private readonly ILogger _logger;
        private readonly IErrorOutcomeBuilder _errorOutcomeBuilder;

        public PageController(IPageCaller pageCaller,
            ILogger logger,
            IErrorOutcomeBuilder errorOutcomeBuilder)
        {
            _pageCaller = pageCaller;
            _logger = logger;
            _errorOutcomeBuilder = errorOutcomeBuilder;
        }

        [HttpGet]
        [Produces("text/html")]
        public async Task<IActionResult> Display()
        {
            Response.Headers.Add("X-Clacks-Overhead", "GNU Terry Pratchett");

            try
            {
                return await _pageCaller.Call(Request);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, Lib.Definitions.LogCategory.Error, Request);

                try
                {
                    return _errorOutcomeBuilder.Build(HttpStatusCode.InternalServerError, Request.HttpContext);
                }
                catch (Exception e)
                {
                    _logger.Log(e.Message, Lib.Definitions.LogCategory.Error, Request);
                    return StatusCode(500);
                }
            }
        }
    }
}
