using System.Net;
using Microsoft.AspNetCore.Mvc;
using SBN.Lib.DB;
using SBN.Lib.IO;

namespace SBN.Lib.Asset
{
    public class AssetRetriever : IAssetRetriever
    {
        private readonly IAssetProcessor _assetProcessor;
        private readonly IFileSystemFacade _fileSystemFacade;
        private readonly IAssets _assets;
        private readonly ISessionTokenAccessor _sessionTokenAccessor;

        public AssetRetriever(IAssetProcessor assetProcessor,
            IFileSystemFacade fileSystemFacade,
            IAssets assets,
            ISessionTokenAccessor sessionTokenAccessor)
        {
            _assetProcessor = assetProcessor;
            _fileSystemFacade = fileSystemFacade;
            _assets = assets;
            _sessionTokenAccessor = sessionTokenAccessor;
        }

        public IActionResult Retrieve(AssetResponse assetResult)
        {
            if (assetResult == null) return new NotFoundResult();

            if (assetResult.AssetCallStatus != HttpStatusCode.OK) return new StatusCodeResult((int)assetResult.AssetCallStatus);

            if (string.IsNullOrWhiteSpace(assetResult.VariantPath))
            {
                assetResult = _assetProcessor.GenerateVariant(assetResult);
                _assets.UpdateVariantFilePath(_sessionTokenAccessor.SessionToken(), assetResult);
            }

            return new FileStreamResult(_fileSystemFacade.LoadAsStream(assetResult.AssetVariantServerPath), assetResult.MIMEType);
        }
    }
}
