using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace SBN.Lib.Drawing
{
    public interface IImageProcessor
    {
        Image ScaleImageToBox(Image image, int width, int height);
        Image ScaleImageToBox(Image image, int width, int height, InterpolationMode interpolationMode);
        Image ResizeImage(Image image, int width, int height);
        Image ResizeImage(Image image, int width, int height, InterpolationMode interpolationMode);
        Image ResizeImage(Image image, ImageResizeMode resizeMode, int targetSize);
        Image ResizeImage(Image image, ImageResizeMode resizeMode, int targetSize, InterpolationMode interpolationMode);
        ImageCodecInfo GetImageCodecInfo(ImageFormat imageFormat);
        EncoderParameters GetParameterJPEGQuality(long quality);
        EncoderParameters GetParameterJPEGQuality(long quality, int encoderParameters);
        Image LoadImage(string path);
    }
}