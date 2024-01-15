using System;
using System.IO;
using System.Net;

namespace SBN.Lib.Asset
{
    public class AssetResponse
    {
        public HttpStatusCode AssetCallStatus { get; set; }
        public int AssetID { get; set; }
        public int AssetVariantID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AssetBasePath { get; set; }
        public string BasePath { get; set; }
        public string BaseVariantPath { get; set; }
        public string VariantPath { get; set; }
        public string VariantParameters { get; set; }
        public string VariantSuffix { get; set; }
        public bool Attachment { get; set; }
        public string ClientFilename { get; set; }
        public string MIMEType { get; set; }
        public DateTime LastUpdated { get; set; }

        public string AssetBaseServerPath 
        {  
            get 
            {
                //return Path.Combine(AssetBasePath, BasePath, BaseVariantPath); For some reason this dropped AssetBasePath consistently?!

                return $@"{AssetBasePath}{Path.DirectorySeparatorChar}{BasePath}{Path.DirectorySeparatorChar}{BaseVariantPath}";
            } 
        }

        public string AssetVariantServerPath 
        { 
            get 
            {
                return $@"{AssetBasePath}{Path.DirectorySeparatorChar}{BasePath}{Path.DirectorySeparatorChar}{VariantPath}";
            }
        }
    }
}
