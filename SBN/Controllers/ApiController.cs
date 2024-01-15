using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SBN.Lib.Action;
using SBN.Lib.Action.JsonConvert;
using SBN.Lib.Action.Outcome;
using SBN.Lib.Definitions;
using SBN.Lib.Request;
using SBN.Lib.Sys;

namespace SBN.Controllers
{
    public class ApiController : Controller
    {
        private readonly IApiActionCaller _apiActionCaller;
        private readonly IAcceptsReader _acceptsReader;
        private readonly IJsonConverter _jsonConverter;
        private readonly IResourceURLGenerator _resourceURLGenerator;
        private readonly ILogger _logger;
        private readonly IXMLSerializerNoNamespace _xmlSerializerNoNamespace;

        public ApiController(IApiActionCaller apiActionCaller,
            IAcceptsReader acceptsReader,
            IJsonConverter jsonConverter,
            IResourceURLGenerator resourceURLGenerator,
            ILogger logger,
            IXMLSerializerNoNamespace xmlSerializerNoNamespace)
        {
            _apiActionCaller = apiActionCaller;
            _acceptsReader = acceptsReader;
            _jsonConverter = jsonConverter;
            _resourceURLGenerator = resourceURLGenerator;
            _logger = logger;
            _xmlSerializerNoNamespace = xmlSerializerNoNamespace;
        }

        public async Task<IActionResult> Call()
        {
            try
            {
                var actionResult = await _apiActionCaller.Call(Request);

                var resourceURL = _resourceURLGenerator.Generate(Request.Path, Request.Query, actionResult.Body.APIRoute, actionResult.Body.OutputParameters);

                if (!string.IsNullOrWhiteSpace(resourceURL)) Response.Headers.Add("X-Resource-URL", resourceURL);
                if (!string.IsNullOrWhiteSpace(actionResult.Body.Message)) Response.Headers.Add("X-Message", actionResult.Body.Message);
                if (!string.IsNullOrWhiteSpace(actionResult.Body.MessageType.ToString())) Response.Headers.Add("X-MessageType", actionResult.Body.MessageType.ToString());

                Response.Headers.Add("X-Clacks-Overhead", "GNU Terry Pratchett");

                var result = actionResult.Body;
                if (string.IsNullOrWhiteSpace(result.Message) && string.IsNullOrWhiteSpace(result.Url))
                {
                    var accept = _acceptsReader.Read(Request);

                    switch (accept)
                    {
                        case ResponseDataType.Xml:
                            return StatusCode((int)actionResult.Body.StatusCode, result.Data);

                        case ResponseDataType.Json:
                            var jsonResult = _jsonConverter.Convert(result);

                            return new ContentResult
                            {
                                StatusCode = (int)actionResult.Body.StatusCode,
                                Content = jsonResult,
                                ContentType = "application/json"
                            };

                        default:
                            return StatusCode((int)HttpStatusCode.BadRequest, Messages.RequestedContentTypeUnavailable);
                    }
                }

                if(!((int)actionResult.Body.StatusCode).Between(200, 399))
                {
                    _logger.Log(actionResult.Body.Message, LogCategory.Error, Request);
                }

                return StatusCode((int)actionResult.Body.StatusCode, _xmlSerializerNoNamespace.Serialize(result));
            }
            catch (Exception ex)        //TODO this has started producing a connection reset in postman rather than returning the actual error message? Fix.
            {
                _logger.Log(ex.Message, LogCategory.Error, Request);

                var response = new Models.Response
                {
                    Message = ex.Message,
                    MessageType = OutcomeMessageType.Error,
                    StatusCode = HttpStatusCode.InternalServerError
                };

                return StatusCode((int)HttpStatusCode.InternalServerError, _xmlSerializerNoNamespace.Serialize(response));
            }
        }
    }
}
