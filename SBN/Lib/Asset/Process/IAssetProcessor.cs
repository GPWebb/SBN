namespace SBN.Lib.Asset
{
    public interface IAssetProcessor
    {
        AssetResponse GenerateVariant(AssetResponse assetResult);
    }
}