using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SBN.Lib;
using SBN.Lib.Definitions;
using SBN.Lib.Sys;
using SBN.Models;

namespace SBN.Controllers
{
    public class SessionController : Controller
    {
        private readonly ISessionTokenAccessor _sessionAccessor;
        private readonly ILogger _logger;

        public SessionController(ISessionTokenAccessor sessionAccessor, ILogger logger)
        {
            _sessionAccessor = sessionAccessor;
            _logger = logger;
        }

        [Produces("application/xml")]
        public async Task<IActionResult> New()
        {
            if (Request.Method != "POST") return StatusCode((int)HttpStatusCode.MethodNotAllowed);

            try
            {
                return StatusCode((int)HttpStatusCode.Created, new NewSession
                {
                    SBNSessionToken = await _sessionAccessor.SetSessionToken()
                });
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogCategory.Error, Request);

                return StatusCode((int)HttpStatusCode.InternalServerError, new Response { Message = ex.Message, MessageType = OutcomeMessageType.Error });
            }
        }
    }
}
