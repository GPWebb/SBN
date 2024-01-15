using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using SBN.Lib.DB;
using SBN.Lib.Definitions;
using SBN.Lib.Page.Merge;
using SBN.Lib.Page.Merge.TokenReplace;
using SBN.Lib.Page.Outcome;
using SBN.Lib.Sys;
using System;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SBN.Lib.Page.Call
{
    public class PageFromCache : IPageFromCache
    {
        private readonly ISession _session;
        private readonly IErrorWrapper _errorWrapper;
        private readonly IHttpResult _httpResult;
        private readonly IPermissionsApplier _permissionsApplier;
        private readonly IStandardParameters _standardParameters;
        private readonly IProfileDataCacher _profileDataCacher;

        public PageFromCache(ISession session,
            IErrorWrapper errorWrapper,
            IHttpResult httpResult,
            IPermissionsApplier permissionsApplier,
            IStandardParameters standardParameters,
            IProfileDataCacher profileDataCacher)
        {
            _session = session;
            _errorWrapper = errorWrapper;
            _httpResult = httpResult;
            _permissionsApplier = permissionsApplier;
            _standardParameters = standardParameters;
            _profileDataCacher = profileDataCacher;
        }

        public async Task<IActionResult> Get(Guid sessionToken, XElement cachedPageXml)
        {
            var sessionDetails = await _session.SessionDetails(sessionToken);

            _profileDataCacher.CacheProfileData(sessionToken, sessionDetails.ProfileData);

            var cachedPage = _permissionsApplier.Apply(sessionDetails.Permissions, cachedPageXml).ToString();

            _errorWrapper.Wrap("merging standard parameters",
                async () => cachedPage = await _standardParameters.StdParams(
                    cachedPage,
                    ParamEncode.PassThrough,
                    EncodeType.None,
                    pageTitle: null, //Will already be set, don't need again
                    sessionToken,
                    passThrough: false));

            return _httpResult.Generate(cachedPage);
        }
    }
}
