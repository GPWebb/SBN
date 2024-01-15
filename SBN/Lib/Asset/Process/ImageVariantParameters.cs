using SBN.Lib.Drawing;

namespace SBN.Lib.Asset.Process
{
    public class ImageVariantParameters
    {
        public ImageResizeMode ImageResizeMode { get; set; }
        public int Width { get; set; }
        public int ScalePercent { get; set; }
        public int Height { get; set; }
        public long EncodingQuality { get; set; }
    }
}