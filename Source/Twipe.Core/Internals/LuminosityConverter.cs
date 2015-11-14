using System.Drawing;

namespace Twipe.Core.Internals
{
    public sealed class LuminosityConverter : BlackWhiteConverterBase
    {
        private readonly float RedCoefficient = 0.21F;
        private readonly float GreenCoefficient = 0.72F;
        private readonly float BlueCoefficient = 0.07F;

        public LuminosityConverter(Bitmap inputImage) : base(inputImage)
        {
            // do nothing!
        }

        protected override int GetAverageValue(Color pixel)
        {
            return (int)(pixel.R * RedCoefficient + pixel.G * GreenCoefficient + pixel.B * BlueCoefficient) / 3;
        }
    }
}