using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SBN.Lib.Definitions;
using SBN.Lib.Sys;

namespace SBN.Lib.Asset
{
    public class AssetCallUnsupportedtrategy : IAssetCallStrategy
    {
        public async Task<IActionResult> Call(HttpRequest request)
        {
            return new ObjectResult(Messages.RequestedMethodUnavailable) { StatusCode = (int)HttpStatusCode.MethodNotAllowed };
        }

        public bool Selector(string method)
        {
            return !method.IsIn("GET", "DELETE", "PUT" );
        }
    }
}
