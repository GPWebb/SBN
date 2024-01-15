using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SBN.Lib.IO
{
    public interface IStreamConverter
    {
        Stream FromImage(Image image, ImageFormat imageFormat, EncoderParameters encoderParameters);
    }
}