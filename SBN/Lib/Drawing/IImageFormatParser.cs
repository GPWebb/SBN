using System.Drawing.Imaging;

namespace SBN.Lib.Drawing
{
    public interface IImageFormatParser
    {
        ImageFormat ParseMIMEType(string mimeType);
    }
}