using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace SBN.Lib.Drawing
{
    public class ImageProcessor : IImageProcessor
    {

        public Image ScaleImageToBox(Image image, int width, int height)
        {
            return ScaleImageToBox(image, width, height, InterpolationMode.HighQualityBicubic);
        }

        public Image ScaleImageToBox(Image image, int width, int height, InterpolationMode interpolationMode)
        {
            double widthFactor = (double)image.Width / width;
            double heightFactor = (double)image.Height / height;

            if ((widthFactor > 1) | (heightFactor > 1))
            {
                return (widthFactor > heightFactor && !double.IsInfinity(widthFactor))
                    ? ResizeImage(image, (int)(image.Width / widthFactor), (int)(image.Height / widthFactor), interpolationMode)
                    : ResizeImage(image, (int)(image.Width / heightFactor), (int)(image.Height / heightFactor), interpolationMode);
            }
            else
            {
                return (widthFactor < heightFactor && widthFactor > 0)
                    ? ResizeImage(image, (int)(image.Width / widthFactor), (int)(image.Height / widthFactor), interpolationMode)
                    : ResizeImage(image, (int)(image.Width / heightFactor), (int)(image.Height / heightFactor), interpolationMode);
            }
        }

        public Image ResizeImage(Image image, int width, int height)
        {
            return ResizeImage(image, width, height, InterpolationMode.HighQualityBicubic);
        }

        public Image ResizeImage(Image image, int width, int height, InterpolationMode interpolationMode)
        {
            Bitmap b = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(b);

            g.InterpolationMode = interpolationMode;
            g.DrawImage(image, 0, 0, width, height);
            g.Dispose();

            return b;
        }

        public Image ResizeImage(Image image, ImageResizeMode resizeMode, int targetSize)
        {
            return ResizeImage(image, resizeMode, targetSize, InterpolationMode.HighQualityBicubic);
        }

        public Image ResizeImage(Image image, ImageResizeMode resizeMode, int targetSize, InterpolationMode interpolationMode)
        {
            float height = image.Height;
            float width = image.Width;

            switch (resizeMode)
            {
                case ImageResizeMode.FitHeight:
                    return ResizeImage(image, (int)(width * (targetSize / height)), targetSize, interpolationMode);

                case ImageResizeMode.FitWidth:
                    return ResizeImage(image, targetSize, (int)(height * (targetSize / width)), interpolationMode);

                case ImageResizeMode.ScalePercent:
                    return ResizeImage(image, (int)(height * targetSize / 100), (int)(height * targetSize / 100), interpolationMode);

                default:
                    throw new Exception("Invalid resize mode specified (it's not supposed to be possible to get this)");
            }
        }

        public ImageCodecInfo GetImageCodecInfo(ImageFormat imageFormat)
        {
            ImageCodecInfo[] l_oEncs = ImageCodecInfo.GetImageEncoders();

            foreach (ImageCodecInfo l_oCodec in l_oEncs)
                if (l_oCodec.FormatID == imageFormat.Guid)
                    return l_oCodec;
            return null;
        }

        public EncoderParameters GetParameterJPEGQuality(long quality)
        {
            return GetParameterJPEGQuality(quality, 1);
        }

        public EncoderParameters GetParameterJPEGQuality(long quality, int encoderParameters)
        {
            EncoderParameters encParams = new EncoderParameters(encoderParameters);
            encParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);

            return encParams;
        }

        public Image LoadImage(string path)
        {
            FileStream pic = new FileStream(path, FileMode.Open);
            Image ret = Image.FromStream(pic);
            pic.Close();

            return ret;
        }
    }
}
