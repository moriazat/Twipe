using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Twipe.Core.Internals
{
    public class AbsolutePixelIdentifier : PixelIdentifierBase
    {
        private unsafe byte* CurrentBytePointer { get; set; }

        public override void IdentifyPixels()
        {
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                                                  ImageLockMode.ReadOnly, image.PixelFormat);

            int imageWidth = image.Width;
            int imageHeight = image.Height;

            unsafe
            {
                CurrentBytePointer = (byte*)imageData.Scan0;

                for (int y = 0; y < imageHeight; y++)
                {
                    for (int x = 0; x < imageWidth; x++)
                    {
                        if (IsCurrentPixelWhite())
                            whites++;
                        else
                            blacks++;

                        CurrentBytePointer += 4;
                    }
                }
            }

            image.UnlockBits(imageData);
        }

        private bool IsCurrentPixelWhite()
        {
            unsafe
            {
                byte* ptr = CurrentBytePointer;

                if ((*ptr == 255) && (*(ptr + 1) == 255) && (*(ptr + 2) == 255))
                    return true;
                else
                    return false;
            }
        }
    }
}