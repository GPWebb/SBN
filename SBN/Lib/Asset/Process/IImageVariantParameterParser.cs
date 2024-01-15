namespace SBN.Lib.Asset.Process
{
    public interface IImageVariantParameterParser
    {
        ImageVariantParameters Parse(string variantParameters);
    }
}