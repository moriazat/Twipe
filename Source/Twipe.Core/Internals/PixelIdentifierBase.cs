using System.Drawing;

namespace Twipe.Core.Internals
{
    public abstract class PixelIdentifierBase : IPixelIdentifier
    {
        protected Bitmap image;
        protected int totalPixels;
        protected double blacks;
        protected double whites;

        public Bitmap Image
        {
            set
            {
                image = value;
                Reset();
            }
        }

        public int TotalPixels
        {
            get { return totalPixels; }
        }

        public int WhitePixels
        {
            get { return (int)whites; }
        }

        public int BlackPixels
        {
            get { return (int)blacks; }
        }

        public double WhiteRatio
        {
            get { return whites / totalPixels; }
        }

        public double BlackRatio
        {
            get { return blacks / totalPixels; }
        }

        public double WhiteToBlackRatio
        {
            get { return whites / blacks; }
        }

        public double BlackToWhiteRatio
        {
            get { return blacks / whites; }
        }

        public abstract void IdentifyPixels();

        protected virtual void Reset()
        {
            totalPixels = image.Width * image.Height;
            blacks = 0;
            whites = 0;
        }
    }
}