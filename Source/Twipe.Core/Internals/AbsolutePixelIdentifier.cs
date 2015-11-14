using System.Drawing;

namespace Twipe.Core.Internals
{
    public class AbsolutePixelIdentifier : PixelIdentifierBase
    {
        public override void IdentifyPixels()
        {
            Color pixel;

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    pixel = image.GetPixel(x, y);
                    if (pixel.ToArgb() == Color.White.ToArgb())
                        whites++;
                    else if (pixel.ToArgb() == Color.Black.ToArgb())
                        blacks++;
                }
            }
        }
    }
}