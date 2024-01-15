using System;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SBN.Lib.DB;
using SBN.Lib.IO;

namespace SBN.Lib.Asset
{
    public class AssetCallDeleteStrategy : IAssetCallStrategy
    {
        private readonly IAssets _assets;
        private readonly ISessionTokenAccessor _sessionTokenAccessor;
        private readonly IFileSystemFacade _fileSystemFacade;

        public AssetCallDeleteStrategy(IAssets assets,
            ISessionTokenAccessor sessionTokenAccessor,
            IFileSystemFacade fileSystemFacade)
        {
            _assets = assets;
            _sessionTokenAccessor = sessionTokenAccessor;
            _fileSystemFacade = fileSystemFacade;
        }

        public bool Selector(string method)
        {
            return method == "DELETE";
        }

        public async Task<IActionResult> Call(HttpRequest request)
        {
            try
            {
                var assetDeleteResult = await _assets.Delete(_sessionTokenAccessor.SessionToken(), request.Path);

                if (assetDeleteResult.StatusCode != HttpStatusCode.OK) return new StatusCodeResult((int)assetDeleteResult.StatusCode);

                foreach (var path in assetDeleteResult.FilePaths)
                {
                    _fileSystemFacade.Delete(path);
                }

                return new NoContentResult();

            }
            catch (SqlException ex)
            {
                var errorNumber = ex.Number.ToString();
                if (errorNumber.Length == 6 && errorNumber.StartsWith("580"))
                {
                    return new StatusCodeResult(int.Parse(errorNumber.Substring(3)));
                }

                return new ObjectResult(ex.Message) { StatusCode = 500 };
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message) { StatusCode = 500 };
            }
        }
    }
}
