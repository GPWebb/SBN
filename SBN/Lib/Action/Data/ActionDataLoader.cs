using SBN.Lib.Action.Outcome;
using System;
using System.Threading.Tasks;

namespace SBN.Lib.Action.Data
{
    public class ActionDataLoader : IActionDataLoader
    {
        private readonly IApiDataLoader _apiDataLoader;

        public ActionDataLoader(IApiDataLoader apiDataLoader)
        {
            _apiDataLoader = apiDataLoader;
        }

        public async Task<ActionData> Load(ActionData actionData,
            Guid sessionToken,
            string requestPath)
        {
            var outcome = new ApiActionOutcome
            {
                SessionToken = sessionToken,
                Body = await _apiDataLoader.Get(
                        actionData,
                        sessionToken,
                        requestPath)
            };

            actionData.Outcome = outcome;

            return actionData;
        }
    }
}
