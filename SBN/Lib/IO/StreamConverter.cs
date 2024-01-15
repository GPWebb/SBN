using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using SBN.Lib.Drawing;

namespace SBN.Lib.IO
{
    public class StreamConverter : IStreamConverter
    {
        private readonly IImageCodecFromImageFormat _imageCodecFromImageFormat;

        public StreamConverter(IImageCodecFromImageFormat imageCodecFromImageFormat)
        {
            _imageCodecFromImageFormat = imageCodecFromImageFormat;
        }

        public Stream FromImage(Image image, ImageFormat imageFormat, EncoderParameters encoderParameters)
        {
            var stream = new MemoryStream();

            var imageCodec = _imageCodecFromImageFormat.Get(imageFormat);

            image.Save(stream, imageCodec, encoderParameters);
            stream.Position = 0;
            return stream;
        }
    }
}
