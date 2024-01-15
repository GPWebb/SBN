using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SBN.Lib;
using SBN.Lib.Asset;
using SBN.Lib.DB;
using SBN.Lib.Definitions;
using SBN.Lib.Sys;

namespace SBN.Controllers
{
    public class AssetController : Controller
    {
        private readonly IEnumerable<IAssetCallStrategy> _assetCallStrategies;
        private readonly ILogger _logger;
        private readonly IAssets _assets;
        private readonly ISessionTokenAccessor _sessionTokenAccessor;
        private readonly IAssetRetriever _assetRetriever;

        public AssetController(IEnumerable<IAssetCallStrategy> assetCallStrategies,
            ILogger logger,
            IAssets assets,
            ISessionTokenAccessor sessionTokenAccessor,
            IAssetRetriever assetRetriever)
        {
            _assetCallStrategies = assetCallStrategies;
            _logger = logger;
            _assets = assets;
            _sessionTokenAccessor = sessionTokenAccessor;
            _assetRetriever = assetRetriever;
        }

        //TODO HUGE can of worms in here, variant names in the client filename aren't being properly generated or parsed. Bodged around for now, needs fixing
        public async Task<IActionResult> Call()
        {
            try
            {
                return await _assetCallStrategies
                    .Single(x => x.Selector(Request.Method.ToUpperInvariant()))
                    .Call(Request);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogCategory.Error, Request);

                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new Models.Response
                    {
                        Message = ex.Message,
                        MessageType = OutcomeMessageType.Error
                    });
            }
        }

        public async Task<IActionResult> CallByIDAndVariant(int ID, string variantSuffix)
        {
            if (Request.Method != "GET") return StatusCode((int)HttpStatusCode.MethodNotAllowed);

            var asset = await _assets.GetByIDAndVariant(_sessionTokenAccessor.SessionToken(), ID, variantSuffix);

            return _assetRetriever.Retrieve(asset);
        }
    }
}
