using System.IO;

namespace SBN.Lib.Asset.Process
{
    public interface IAssetVariantPersister
    {
        AssetResponse Persist(AssetResponse assetResult, Stream stream);
    }
}