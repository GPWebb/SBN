using System.Drawing.Imaging;

namespace SBN.Lib.Drawing
{
    public class ImageCodecFromImageFormat : IImageCodecFromImageFormat
    {
        //https://docs.microsoft.com/en-us/dotnet/desktop/winforms/advanced/how-to-set-jpeg-compression-level - No, I don't like it either. Try to find a better way.
        public ImageCodecInfo Get(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
