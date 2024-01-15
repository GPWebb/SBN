using System.Net;

namespace SBN.Lib.Asset
{
    public class AssetCreatedResponse
    {
        public HttpStatusCode StatusCode;
        public int AssetID;
        public string Path;
        public string ErrorMessage;
    }
}