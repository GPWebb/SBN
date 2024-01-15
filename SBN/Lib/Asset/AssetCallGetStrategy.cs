using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SBN.Lib.DB;

namespace SBN.Lib.Asset
{
    public class AssetCallGetStrategy : IAssetCallStrategy
    {
        private readonly ISessionTokenAccessor _sessionTokenAccessor;
        private readonly IAssets _assets;
        private readonly IAssetRetriever _assetRetriever;

        public AssetCallGetStrategy(ISessionTokenAccessor sessionTokenAccessor,
            IAssets assets,
            IAssetRetriever assetRetriever)
        {
            _sessionTokenAccessor = sessionTokenAccessor;
            _assets = assets;
            _assetRetriever = assetRetriever;
        }

        public bool Selector(string method)
        {
            return method == "GET";
        }

        public async Task<IActionResult> Call(HttpRequest request)
        {
            var assetResult = await _assets.Get(_sessionTokenAccessor.SessionToken(), request.Path);

            return _assetRetriever.Retrieve(assetResult);
        }
    }
}
