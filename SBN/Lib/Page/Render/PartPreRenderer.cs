using SBN.Lib.Action.Data;
using SBN.Lib.Action.Outcome;
using SBN.Lib.Page.Merge;
using SBN.Lib.Page.Request;
using SBN.Lib.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SBN.Lib.Page.Render
{
    public class PartPreRenderer : IPartPreRenderer
    {
        private readonly IPermissionsApplier _permissionsApplier;
        private readonly IApiActionOutcomeDataPopulator _apiActionOutcomeBuilder;
        private readonly IPagePartRenderer _pagePartRenderer;
        private readonly INonDataMerger _nonDataMerger;
        private readonly ISessionTokenAccessor _sessionTokenAccessor;

        public PartPreRenderer(IPermissionsApplier permissionsApplier,
            IApiActionOutcomeDataPopulator apiActionOutcomeBuilder,
            IPagePartRenderer pagePartRenderer,
            INonDataMerger nonDataMerger,
            ISessionTokenAccessor sessionTokenAccessor)
        {
            _permissionsApplier = permissionsApplier;
            _apiActionOutcomeBuilder = apiActionOutcomeBuilder;
            _pagePartRenderer = pagePartRenderer;
            _nonDataMerger = nonDataMerger;
            _sessionTokenAccessor = sessionTokenAccessor;
        }

        public async Task<XElement> PreRenderPart(XElement pageTemplate,
            IEnumerable<string> permissions,
            IEnumerable<ActionData> actionData,
            PagePart part,
            string pageTitle,
            ValidatedRequest requestParams)
        {
            part.PartXML = _permissionsApplier.Apply(permissions, part.PartXML);
            part.Transform = _permissionsApplier.Apply(permissions, part.Transform);
            part.Transform = await _nonDataMerger.MergeNonDataXml(part.Transform,
                pageTitle,
                Definitions.ParamEncode.PassThrough,
                Definitions.EncodeType.None,
                _sessionTokenAccessor.SessionToken(),
                requestParams);

            if (!(part.Source_ActionID == null || (part.DelayLoad ?? false)))
            {
                var actions = actionData.Where(x => x.ActionID == part.Source_ActionID);
                if (actions.Count() > 1) actions = actions.Where(x => x.ActionParameters == part.ActionParameters);

                switch (actions.Count())
                {
                    case 1:
                        var outcome = actions.Single().Outcome;

                        var statusCode = outcome?
                            .Body?
                            .StatusCode ?? HttpStatusCode.InternalServerError;

                        if (!(statusCode.IsSuccessStatusCode()
                            || statusCode == HttpStatusCode.NotFound
                            || (statusCode == 0)))
                        //if (!statusCode.IsSuccessStatusCode())
                        {
                            throw new Exception($"Requested action '{part.Source_ActionID}' returned response code '{statusCode}'");
                        }
                        var data = ReadActionData(actions, outcome);

                        pageTemplate = _pagePartRenderer.RenderPart(pageTemplate, part, data);
                        return pageTemplate;


                    case 0:
                        throw new Exception($"Requested action '{part.Source_ActionID}' not found in action data. Listed actions: {string.Join(", ", actions.Select(x => x.ActionID))}");

                    default:
                        throw new Exception(
                            $"Requested action '{part.Source_ActionID}' and parameters '{part.ActionParameters}' matches {actions.Count()} datasets in the result data");
                }
            }

            pageTemplate = _pagePartRenderer.RenderPart(pageTemplate, part, null);
            return pageTemplate;
        }

        private XElement ReadActionData(IEnumerable<ActionData> actions, ApiActionOutcome outcome)
        {
            var data = outcome
                .Body
                .Data;

            if (data == null)
            {
                var dataOutcome = _apiActionOutcomeBuilder.Populate(actions.Single(), decorateForJson: false);
                actions.Single().Outcome = outcome;
                data = dataOutcome.Body.Data;
            }

            return data;
        }
    }
}