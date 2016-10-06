using System;
using System.Drawing;

namespace Twipe.Core.Internals
{
    public class AverageConverter : BlackWhiteConverterBase
    {
        public AverageConverter(Bitmap inputImage) : base(inputImage)
        {
            // do nothing!
        }

        protected override int GetAverageValue(Color pixel)
        {
            return (pixel.R + pixel.G + pixel.B) / 3;
        }

        protected override int GetAverageValue(int red, int green, int blue)
        {
            return (red + green + blue) / 3;
        }
    }
}