using System.Collections.Generic;
using System.Net;

namespace SBN.Lib.Asset
{
    public class AssetDeleteResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public IEnumerable<string> FilePaths { get; set; }
    }
}