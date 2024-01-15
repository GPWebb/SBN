using System;
using System.Drawing.Imaging;

namespace SBN.Lib.Drawing
{
    public class ImageFormatParser : IImageFormatParser
    {
        public ImageFormat ParseMIMEType(string mimeType)
        {
            switch (mimeType.ToLowerInvariant())
            {
                case "image/bmp":
                    return ImageFormat.Bmp;

                case "image/gif":
                    return ImageFormat.Gif;

                case "image/jpeg":
                    return ImageFormat.Jpeg;

                case "image/tiff":
                    return ImageFormat.Tiff;

                case "image/png":
                    return ImageFormat.Png;

                default:
                    throw new ArgumentException($"Unsupported MIME type '{mimeType}'");
            }
        }
    }
}
