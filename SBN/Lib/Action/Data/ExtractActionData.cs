using SBN.Lib.Action.Outcome;
using SBN.Models;

namespace SBN.Lib.Action.Data
{
    public class ExtractActionData : IExtractActionData
    {
        private readonly IApiActionOutcomeDataPopulator _apiActionOutcomeDataPopulator;
        private readonly ITransformActionData _transformActionData;

        public ExtractActionData(IApiActionOutcomeDataPopulator apiActionOutcomeDataPopulator,
            ITransformActionData transformActionData)
        {
            _apiActionOutcomeDataPopulator = apiActionOutcomeDataPopulator;
            _transformActionData = transformActionData;
        }

        public Response ResponseFromActionData(ActionData result, bool transform = false)
        {
            var outcome = _apiActionOutcomeDataPopulator.Populate(result, decorateForJson: false, !transform);  //HACK don't like transform being handled like this, suspect data loading needs rearchitecting

            if (transform && outcome.Body.Data != null) _transformActionData.Transform(result, outcome);

            return outcome.Body;
        }
    }
}
