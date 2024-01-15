using System;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SBN.Lib.Action.Outcome;
using SBN.Lib.Definitions;
using SBN.Lib.Request;
using SBN.Lib.Sys;

namespace SBN.Lib.Action
{
    public class ApiActionCaller : IApiActionCaller
    {
        private readonly IOptions<EnvironmentConfig> _environmentConfig;
        private readonly IApiActionDbCaller _apiActionDbCaller;
        private readonly IRequestReader _requestReader;
        private readonly IRequestStreamReader _requestStreamReader;
        private readonly ISessionTokenAccessor _sessionAccessor;
        private readonly IAcceptsReader _acceptsReader;
        private readonly ILogger _logger;

        public ApiActionCaller(IOptions<EnvironmentConfig> environmentConfig,
            IApiActionDbCaller apiActionDbCaller,
            IRequestReader requestReader,
            IRequestStreamReader requestStreamReader,
            ISessionTokenAccessor sessionAccessor,
            IAcceptsReader acceptsReader,
            ILogger logger)
        {
            _environmentConfig = environmentConfig;
            _apiActionDbCaller = apiActionDbCaller;
            _requestReader = requestReader;
            _requestStreamReader = requestStreamReader;
            _sessionAccessor = sessionAccessor;
            _acceptsReader = acceptsReader;
            _logger = logger;
        }

        public async Task<ApiActionOutcome> Call(HttpRequest request)
        {
            var sessionToken = _sessionAccessor.SessionToken();

            if (_environmentConfig.Value.IndividualUnauthenticatedSessions)
            {
                if (sessionToken == Guid.Empty)
                {
                    return new ApiActionOutcome(HttpStatusCode.BadRequest,
                        Messages.NoSessionToken,
                        OutcomeMessageType.Error,
                        $"/{Routes.NewSession}");
                }
            }

            XDocument requestBody;
            try
            {
                requestBody = _requestStreamReader.Read(request.Body);
            }
            catch (Exception ex)
            {
                _logger.Log($"Error reading request body for {request.Path}: {ex.Message}", LogCategory.Error, request);

                return new ApiActionOutcome(HttpStatusCode.BadRequest,
                    Messages.RequestBodyUnreadable,
                    OutcomeMessageType.Error);
            }

            var path = request.Path.ToString();
            var query = request.QueryString.ToUriComponent();

            if (path.LastIndexOf(".") > path.LastIndexOf("/"))
            {
                path = path.Substring(0, path.LastIndexOf("."));
            }

            var pathAndQuery = path + query;

            var verb = request.Method;
            var referrer = _requestReader.Referrer(request);
            var decorateForJson = (_acceptsReader.Read(request) == ResponseDataType.Json);

            return await _apiActionDbCaller.Call(sessionToken, pathAndQuery, verb, referrer, decorateForJson, requestBody);
        }
    }
}
