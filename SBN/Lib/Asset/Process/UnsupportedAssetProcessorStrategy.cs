using System;
using SBN.Lib.Sys;

namespace SBN.Lib.Asset.Process
{
    public class UnsupportedAssetProcessorStrategy : IAssetProcessorStrategy
    {
        public bool Select(string MIMEType)
        {
            return !MIMEType.IsIn("image/bmp", "image/gif", "image/jpeg", "image/tiff", "image/png");
        }

        public AssetResponse GenerateVariant(AssetResponse assetResult)
        {
            throw new ArgumentException($"MIME Type '{assetResult.MIMEType}' not processable");
        }
    }
}
