using Microsoft.AspNetCore.Http;

namespace SBN.Lib.Asset
{
    public class AssetPutRequest
    {
        public string AssetType;
        public string AssetPath;
        public string Name;
        public string Description;
        public IFormFile File;
        public string ClientFilename;
        public string MIMEType;
    }
}