using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using SBN.Lib.Page.Outcome.ErrorOutcome;
using SBN.Lib.Page.Render;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SBN.Lib.Page.Outcome
{
    public class PageOutcome : IPageOutcome
    {
        private readonly IPageRenderer _pageRenderer;
        private readonly IProfileDataCacher _profileDataCacher;
        private readonly ISessionTokenAccessor _sessionTokenAccessor;
        private readonly IDynamicPagePartPopulator _dynamicPagePartPopulator;
        private readonly IPageDocumentChecker _pageDocumentChecker;
        private readonly IDocumentDataSourcePopulator _documentDataSourcePopulator;
        private readonly IErrorOutcomeBuilder _errorOutcomeBuilder;
        private readonly IHttpResult _httpResult;

        public PageOutcome(IPageRenderer pageRenderer,
            IProfileDataCacher profileDataCacher,
            ISessionTokenAccessor sessionTokenAccessor,
            IDynamicPagePartPopulator dynamicPagePartPopulator,
            IPageDocumentChecker pageDocumentChecker,
            IDocumentDataSourcePopulator documentDataSourcePopulator,
            IErrorOutcomeBuilder errorOutcomeBuilder,
            IHttpResult httpResult)
        {
            _pageRenderer = pageRenderer;
            _profileDataCacher = profileDataCacher;
            _sessionTokenAccessor = sessionTokenAccessor;
            _dynamicPagePartPopulator = dynamicPagePartPopulator;
            _pageDocumentChecker = pageDocumentChecker;
            _documentDataSourcePopulator = documentDataSourcePopulator;
            _errorOutcomeBuilder = errorOutcomeBuilder;
            _httpResult = httpResult;
        }

        public HttpStatusCode HttpStatusCode { get; set; }

        public async Task<IActionResult> Build(PageData pageData, HttpContext context)
        {
            _profileDataCacher.CacheProfileData(_sessionTokenAccessor.SessionToken(), pageData.ProfileData);

            var parts = pageData.PageParts;

            var pageDocuments = pageData.PageDocuments;

            if (pageDocuments != null) pageDocuments = await (_documentDataSourcePopulator.Populate(pageDocuments)).ToListAsync();

            var documentTypeResponse = pageData.DocumentTypeResponse;

            var statusCode = _pageDocumentChecker.Check(pageDocuments, documentTypeResponse);

            if (statusCode != null) return _errorOutcomeBuilder.Build((HttpStatusCode)statusCode, context);

            var actionData = pageData.ActionData.ToList();

            parts = _dynamicPagePartPopulator.Populate(parts, actionData).ToList();

            HttpStatusCode = HttpStatusCode.OK;

            var body = await _pageRenderer
                .Render(pageData.Template,
                    pageData.Title,
                    pageData.PageReferences,
                    pageData.PagePermissions,
                    parts,
                    _sessionTokenAccessor.SessionToken(),
                    context.Request.Path,
                    UriHelper.GetEncodedPathAndQuery(context.Request),
                    pageData.ValidatedRequest,
                    actionData,
                    pageDocuments,
                    pageData.CacheExpiry);

            return _httpResult.Generate(body);
        }
    }
}
