using System;
using System.Web;
using SBN.Lib.Drawing;
using SBN.Lib.Sys;

namespace SBN.Lib.Asset.Process
{
    public class ImageVariantParameterParser : IImageVariantParameterParser
    {
        public ImageVariantParameters Parse(string variantParameters)
        {
            if (string.IsNullOrWhiteSpace(variantParameters)) return null;

            var parsedParameters = new ImageVariantParameters();

            var parameters = HttpUtility.ParseQueryString(variantParameters).ToTuples();

            foreach (var parameter in parameters)
            {
                switch (parameter.name)
                {
                    case "ImageResizeMode":
                        parsedParameters.ImageResizeMode = Enum.Parse<ImageResizeMode>(parameter.value);
                        break;

                    case "Width":
                        parsedParameters.Width = int.Parse(parameter.value);
                        break;

                    case "Height":
                        parsedParameters.Height = int.Parse(parameter.value);
                        break;

                    case "ScalePercent":
                        parsedParameters.ScalePercent = int.Parse(parameter.value);
                        break;

                    default:
                        throw new ArgumentException($"Unrecognised image variant parameter '{parameter.name}'");
                }
            }

            return parsedParameters;
        }
    }
}
