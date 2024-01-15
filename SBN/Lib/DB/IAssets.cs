using System;
using System.Threading.Tasks;
using SBN.Lib.Asset;

namespace SBN.Lib.DB
{
    public interface IAssets
    {
        Task<AssetResponse> Get(Guid sessionToken, string assetPath);
        Task<AssetDeleteResponse> Delete(Guid sessionToken, string path);
        Task<AssetCreatedResponse> Add(Guid sessionToken, string assetType, string assetPath, string name, string description, string clientFilename, string MIMEType);
        Task<AssetResponse> GetByIDAndVariant(Guid sessionToken, int ID, string variantSuffix);
        Task UpdateVariantFilePath(Guid sessionToken, AssetResponse assetResult);
    }
}