using Microsoft.AspNetCore.Mvc;

namespace SBN.Lib.Asset
{
    public interface IAssetRetriever
    {
        IActionResult Retrieve(AssetResponse assetResult);
    }
}