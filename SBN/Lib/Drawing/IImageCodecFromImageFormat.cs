using System.Drawing.Imaging;

namespace SBN.Lib.Drawing
{
    public interface IImageCodecFromImageFormat
    {
        ImageCodecInfo Get(ImageFormat format);
    }
}