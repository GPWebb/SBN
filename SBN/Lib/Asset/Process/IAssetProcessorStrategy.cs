namespace SBN.Lib.Asset.Process
{
    public interface IAssetProcessorStrategy
    {
        bool Select(string MIMEType);
        AssetResponse GenerateVariant(AssetResponse assetResult);
    }
}