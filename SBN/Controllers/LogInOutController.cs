using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SBN.Lib;
using SBN.Lib.Analytics;
using SBN.Lib.DB;
using SBN.Lib.Definitions;
using SBN.Lib.Login;
using SBN.Lib.Sys;
using SBN.Models;
using ISettings = SBN.Lib.ISettings;

namespace SBN.Controllers
{
    public class LogInOutController : Controller
    {
        private readonly ILogins _logins;
        private readonly ISessionTokenAccessor _sessionAccessor;
        private readonly ISettings _settings;
        private readonly ILogger _logger;
        private readonly IGeolocator _geolocator;

        public LogInOutController(ILogins logins,
            ISessionTokenAccessor sessionAccessor,
            ISettings settings,
            ILogger logger,
            IGeolocator geolocator)
        {
            _logins = logins;
            _sessionAccessor = sessionAccessor;
            _settings = settings;
            _logger = logger;
            _geolocator = geolocator;
        }

        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (Request.Method != "POST") return StatusCode((int)HttpStatusCode.MethodNotAllowed);

            if (loginRequest == null) return StatusCode((int)HttpStatusCode.BadRequest);

            try
            {
                var response = await DoLogin(loginRequest);

                var statusCode = response.ResultType.IsIn(LoginResponse.LoginResponseType.Success, LoginResponse.LoginResponseType.PasswordMustBeChanged)
                    ? HttpStatusCode.OK
                    : HttpStatusCode.BadRequest;

                return StatusCode((int)statusCode, response);
            }
            catch (Exception ex)
            {
                if (ex.Message == Messages.InvalidSessionToken)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new Response
                    {
                        Message = ex.Message,
                        MessageType = OutcomeMessageType.Error,
                        Url = $"/{Routes.NewSession}"
                    });
                }

                _logger.Log(ex.Message, LogCategory.Error, Request);

                return StatusCode((int)HttpStatusCode.InternalServerError, new Response
                {
                    Message = ex.Message,
                    MessageType = OutcomeMessageType.Error
                });
            }
        }

        private async Task<LoginResponse> DoLogin(LoginRequest loginRequest)
        {
            var loginResponse = await _logins.Login(_sessionAccessor.SessionToken(), loginRequest.Username, loginRequest.Password);

            if (loginResponse.ResultType == LoginResponse.LoginResponseType.SessionMustBeCreated)
            {
                var sessionToken = await _sessionAccessor.SetSessionToken();
                loginResponse = await _logins.Login(sessionToken, loginRequest.Username, loginRequest.Password);
            }
            else
            {
                _geolocator.Set(_sessionAccessor.SessionToken(), HttpContext);
            }

            if (loginResponse.ResultType == LoginResponse.LoginResponseType.Success)
            {
                loginResponse.URL = (_settings.GetSetting("SiteBaseAddress") + loginRequest.URL);
            }

            return loginResponse;
        }

        public IActionResult Logout()
        {
            try
            {
                if (Request.Method != "POST") return StatusCode((int)HttpStatusCode.MethodNotAllowed);

                Guid sessionToken = _sessionAccessor.SessionToken();
                if (sessionToken == Guid.Empty) return BadRequest("Invalid session token");

                _logins.Logout(sessionToken);

                _sessionAccessor.ResetSessionToken();

                return StatusCode((int)HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogCategory.Error, Request);


                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new Response
                    {
                        Message = ex.Message,
                        MessageType = OutcomeMessageType.Error
                    });
            }
        }
    }
}
