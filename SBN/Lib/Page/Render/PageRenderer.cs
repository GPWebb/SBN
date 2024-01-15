using SBN.Lib.Action.Data;
using SBN.Lib.Definitions;
using SBN.Lib.Page.Merge.TokenReplace;
using SBN.Lib.Page.Render.Util;
using SBN.Lib.Page.Request;
using SBN.Lib.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SBN.Lib.Page.Render
{
    public class PageRenderer : IPageRenderer
    {
        private readonly IPermissionsApplier _permissionsApplier;
        private readonly IReferenceApplier _referenceApplier;
        private readonly IDataMerger _dataMerger;
        private readonly IDocumentMerger _documentMerger;
        private readonly ISiteTextMerger _siteTextMerger;
        private readonly ISBNContentRenderer _sbnContentRenderer;
        private readonly IPageContentRenderer _pageContentRenderer;
        private readonly IDocumentCleanup _documentCleanup;
        private readonly IErrorWrapper _errorWrapper;
        private readonly IPageCache _pageCache;
        private readonly IQueryStringReplacer _queryStringReplacer;
        private readonly IStandardParameters _standardParameters;

        public PageRenderer(IPermissionsApplier permissionsApplier,
            IReferenceApplier referenceApplier,
            IDataMerger dataMerger,
            IDocumentMerger documentMerger,
            ISiteTextMerger siteTextMerger,
            ISBNContentRenderer sbnContentRenderer,
            IPageContentRenderer pageContentRenderer,
            IDocumentCleanup documentCleanup,
            IErrorWrapper errorWrapper,
            IPageCache pageCache,
            IQueryStringReplacer queryStringReplacer,
            IStandardParameters standardParameters)
        {
            _permissionsApplier = permissionsApplier;
            _referenceApplier = referenceApplier;
            _dataMerger = dataMerger;
            _documentMerger = documentMerger;
            _siteTextMerger = siteTextMerger;
            _sbnContentRenderer = sbnContentRenderer;
            _pageContentRenderer = pageContentRenderer;
            _documentCleanup = documentCleanup;
            _errorWrapper = errorWrapper;
            _pageCache = pageCache;
            _queryStringReplacer = queryStringReplacer;
            _standardParameters = standardParameters;
        }

        public async Task<string> Render(XElement pageTemplate,
            string pageTitle,
            string pageReferences,
            IEnumerable<string> permissions,
            IEnumerable<PagePart> pageParts,
            Guid sessionToken,
            string path,
            string pathAndQuery,
            ValidatedRequest validatedRequest,
            IEnumerable<ActionData> actionData,
            IEnumerable<PageDocument> pageDocuments,
            DateTime? cacheExpiry)
        {
            if (!cacheExpiry.HasValue)
            {
                pageTemplate = _permissionsApplier.Apply(permissions, pageTemplate);
            }

            pageTemplate = _referenceApplier.Apply(pageTemplate, pageReferences, pageParts);

            var maxPart = pageParts.AnyExist()
                ? pageParts
                    .OrderByDescending(x => x.Position)
                    ?.First()
                    ?.Position
                    ?? 0
                : 0;
            var maxDocument = pageDocuments.AnyExist()
                ? pageDocuments
                    ?.OrderByDescending(x => x.Position)
                    ?.First()
                    .Position
                    ?? 0
                : 0;
            var maxPosition = maxPart > maxDocument
                ? maxPart
                : maxDocument;

            pageTemplate = await _pageContentRenderer.RenderPageContents(pageTemplate,
                permissions,
                pageParts,
                actionData,
                pageDocuments,
                maxPosition,
                pageTitle,
                validatedRequest);

            pageTemplate = _sbnContentRenderer.Render(pageTemplate);

            var pageTemplateString = pageTemplate.ToString();

            _errorWrapper.Wrap("merging page dataset(s)",
                async () => { if (actionData.AnyExist())
                    {
                        pageTemplateString = await _dataMerger.MergeData(pageTemplateString, actionData, sessionToken, path, validatedRequest);
                        pageTitle = await _dataMerger.MergeData(pageTitle, actionData, sessionToken, path, validatedRequest);
                    }
                });

            _errorWrapper.Wrap("merging page document(s)",
                () => { if (pageDocuments.AnyExist()) pageTemplateString = _documentMerger.MergeDocuments(pageTemplateString, pageDocuments); });

            _errorWrapper.Wrap("merging site text",
                async () =>
                {
                    pageTemplateString = await _siteTextMerger.Merge(pageTemplateString);
                    pageTitle = await _siteTextMerger.Merge(pageTitle);
                });

            _errorWrapper.Wrap("cleaning document",
                () => pageTemplateString = _documentCleanup.Clean(pageTemplateString));

            _errorWrapper.Wrap("merging query string parameters",
                () => pageTemplateString = _queryStringReplacer.Replace(pageTemplateString, validatedRequest, ParamEncode.PassThrough, EncodeType.None));

            _errorWrapper.Wrap("merging standard parameters",
                async () => pageTemplateString = await _standardParameters.StdParams(
                    pageTemplateString, 
                    ParamEncode.PassThrough, 
                    EncodeType.None, 
                    pageTitle, 
                    sessionToken,
                    passThrough: true));

            if (cacheExpiry.HasValue)
            {
                _pageCache.Set($"{pathAndQuery}", pageTemplateString, cacheExpiry.Value);
                pageTemplateString = _permissionsApplier.Apply(permissions, XElement.Parse(pageTemplateString)).ToString();
            }

            return pageTemplateString;
        }
    }
}