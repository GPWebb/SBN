using System.Collections.Generic;
using System.Linq;

namespace SBN.Lib.Asset.Process
{
    public class AssetProcessor : IAssetProcessor
    {
        private readonly IEnumerable<IAssetProcessorStrategy> _assetProcessorStrategies;

        public AssetProcessor(IEnumerable<IAssetProcessorStrategy> assetProcessorStrategies)
        {
            _assetProcessorStrategies = assetProcessorStrategies;
        }

        public AssetResponse GenerateVariant(AssetResponse assetResult)
        {
            return _assetProcessorStrategies
                .Single(x => x.Select(assetResult.MIMEType))
                .GenerateVariant(assetResult);
        }
    }
}
