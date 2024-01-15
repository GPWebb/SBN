using System.IO;
using SBN.Lib.IO;

namespace SBN.Lib.Asset.Process
{
    public class AssetVariantPersister : IAssetVariantPersister
    {
        private readonly IFileSystemFacade _fileSystemFacade;

        public AssetVariantPersister(IFileSystemFacade fileSystemFacade)
        {
            _fileSystemFacade = fileSystemFacade;
        }

        public AssetResponse Persist(AssetResponse assetResult, Stream stream)
        {
            assetResult.VariantPath = Path.GetFileNameWithoutExtension(assetResult.BaseVariantPath)
                                      + "__"
                                      + assetResult.VariantSuffix
                                      + Path.GetExtension(assetResult.BaseVariantPath);

            _fileSystemFacade.SaveStream(assetResult.AssetVariantServerPath, stream);

            return assetResult;
        }
    }
}
