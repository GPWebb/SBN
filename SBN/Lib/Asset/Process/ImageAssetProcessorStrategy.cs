using System;
using System.Drawing.Imaging;
using SBN.Lib.Drawing;
using SBN.Lib.IO;
using SBN.Lib.Sys;

namespace SBN.Lib.Asset.Process
{
    public class ImageAssetProcessorStrategy : IAssetProcessorStrategy
    {
        private readonly IImageProcessor _imageProcessor;
        private readonly IImageVariantParameterParser _imageVariantParameterParser;
        private readonly IStreamConverter _streamConverter;
        private readonly IImageFormatParser _imageFormatParser;
        private readonly IAssetVariantPersister _assetVariantPersister;

        public ImageAssetProcessorStrategy(IImageVariantParameterParser imageVariantParameterParser,
            IImageFormatParser imageFormatParser,
            IImageProcessor imageProcessor,
            IStreamConverter streamConverter,
            IAssetVariantPersister assetVariantPersister)
        {
            _imageVariantParameterParser = imageVariantParameterParser;
            _imageFormatParser = imageFormatParser;
            _imageProcessor = imageProcessor;
            _streamConverter = streamConverter;
            _assetVariantPersister = assetVariantPersister;
        }

        public bool Select(string MIMEType)
        {
            return MIMEType.IsIn("image/bmp", "image/gif", "image/jpeg", "image/tiff", "image/png");
        }

        public AssetResponse GenerateVariant(AssetResponse assetResult)
        {
            var image = _imageProcessor.LoadImage(assetResult.AssetBaseServerPath);

            var variantParameters = _imageVariantParameterParser.Parse(assetResult.VariantParameters);

            switch (variantParameters.ImageResizeMode)
            {
                case ImageResizeMode.FitWidth:
                    image = _imageProcessor.ResizeImage(image, ImageResizeMode.FitWidth, variantParameters.Width);
                    break;

                case ImageResizeMode.FitHeight:
                    image = _imageProcessor.ResizeImage(image, ImageResizeMode.FitHeight, variantParameters.Width);
                    break;

                case ImageResizeMode.FitBox:
                    image = _imageProcessor.ScaleImageToBox(image, variantParameters.Width, variantParameters.Height);
                    break;

                case ImageResizeMode.ScalePercent:
                    image = _imageProcessor.ResizeImage(image, ImageResizeMode.ScalePercent, variantParameters.ScalePercent);
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Resize mode '{variantParameters.ImageResizeMode}' not supported");
            }

            if (variantParameters.EncodingQuality == 0) { variantParameters.EncodingQuality = Definitions.Images.DefaultJpegEncodingQuality; }

            EncoderParameters encoderParameters = new EncoderParameters(0);
            if (assetResult.MIMEType == "image/jpeg")
            {
                Encoder encoder = Encoder.Quality;
                encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(encoder, variantParameters.EncodingQuality);
            }

            assetResult = _assetVariantPersister.Persist(assetResult,
                _streamConverter.FromImage(image,
                    _imageFormatParser.ParseMIMEType(assetResult.MIMEType),
                    encoderParameters));

            return assetResult;
        }
    }
}
