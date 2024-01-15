using SBN.Lib.DB;
using SBN.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SBN.Lib.Action.Data
{
    public class ApiDataLoader : IApiDataLoader
    {
        private readonly IActionDataCache _actionDataCache;
        private readonly IActions _actions;
        private readonly IExtractActionData _extractActionData;

        public ApiDataLoader(IActionDataCache actionDataCache,
            IActions actions,
            IExtractActionData extractActionData)
        {
            _actionDataCache = actionDataCache;
            _actions = actions;
            _extractActionData = extractActionData;
        }

        public async Task<Response> Get(ActionData actionData,
            Guid sessionToken,
            string requestPath)
        {
            if (actionData.CacheExpiry.HasValue)
            {
                //HACK this currently can only cache page data requests, not API
                var cacheKey = actionData.CacheBySession ?? true
                        ? $"{actionData.ActionID}|{actionData.ActionParameters}|{sessionToken}"
                        : $"{actionData.ActionID}|{actionData.ActionParameters}";

                Response response;
                var populated = _actionDataCache.TryGet(cacheKey, out var apiData);

                if(populated)
                {
                    response = new Response
                    {
                        Data = apiData,
                        StatusCode = apiData == null ? HttpStatusCode.NotFound : HttpStatusCode.OK
                    };
                }
                else
                {
                    var result = await _actions.CallByID(sessionToken, actionData.ActionID, requestPath, actionData.ActionParameters);
                    result.ActionParameters = actionData.ActionParameters;
                    result.SourceURL = actionData.SourceURL;
                    result.Definition = actionData.Definition;
                    result.Transform = actionData.Transform;
                    result.CacheExpiry = actionData.CacheExpiry;
                    result.CacheBySession = actionData.CacheBySession;

                    response = _extractActionData.ResponseFromActionData(result, transform: true);

                    _actionDataCache.Set(cacheKey, response.Data, actionData.CacheExpiry.Value);
                }

                return response;
            }
            else
            {
                var result = await _actions.CallByID(sessionToken, actionData.ActionID, requestPath, actionData.ActionParameters);
                result.Definition = actionData.Definition;
                result.Transform = actionData.Transform;

                return _extractActionData.ResponseFromActionData(result, transform: true);
            }
        }
    }
}
