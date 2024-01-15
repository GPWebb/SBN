using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SBN.Lib.DB;
using SBN.Lib.IO;
using SBN.Lib.Sys;

namespace SBN.Lib.Asset
{
    public class AssetCallPutStrategy : IAssetCallStrategy
    {
        private readonly IAssets _assets;
        private readonly ISessionTokenAccessor _sessionTokenAccessor;
        private readonly IFileSystemFacade _fileSystemFacade;
        private readonly IAssetPutRequestParser _assetPutRequestParser;
        private readonly ISettings _settings;
        private readonly ILogger _logger;

        public AssetCallPutStrategy(IAssets assets,
            ISessionTokenAccessor sessionTokenAccessor,
            IFileSystemFacade fileSystemFacade,
            IAssetPutRequestParser assetPutRequestParser,
            ISettings settings,
            ILogger logger)
        {
            _assets = assets;
            _sessionTokenAccessor = sessionTokenAccessor;
            _fileSystemFacade = fileSystemFacade;
            _assetPutRequestParser = assetPutRequestParser;
            _settings = settings;
            _logger = logger;
        }

        public bool Selector(string method)
        {
            return method == "PUT";
        }

        public async Task<IActionResult> Call(HttpRequest request)
        {
            var assetPutRequest = _assetPutRequestParser.Parse(request);

            var assetResponse = await _assets.Add(_sessionTokenAccessor.SessionToken(),
                assetPutRequest.AssetType,
                assetPutRequest.AssetPath,
                assetPutRequest.Name,
                assetPutRequest.Description,
                assetPutRequest.ClientFilename,
                assetPutRequest.MIMEType);

            if (assetResponse.StatusCode != HttpStatusCode.Created) 
            {
                _logger.Log(assetResponse.ErrorMessage, Definitions.LogCategory.Error, request);

                return new ContentResult
                { 
                    StatusCode = (int)assetResponse.StatusCode,
                    Content = assetResponse.ErrorMessage 
                }; 
            }

            var savePath = _settings.GetSetting("AssetBasePath") + assetResponse.Path.Replace("/", "\\");

            _fileSystemFacade.SaveFormFile(savePath, assetPutRequest.File);

            return new CreatedResult(assetResponse.Path, assetResponse);
        }
    }
}
