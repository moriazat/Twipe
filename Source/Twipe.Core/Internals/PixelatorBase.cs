using System.Drawing;

namespace Twipe.Core.Internals
{
    /// <summary>
    /// This is the base class for algorithms that pixelate a given image.
    /// </summary>
    /// <typeparam name="T">Type of the object used for susbstitution of each pixel in the given image.</typeparam>
    public abstract class PixelatorBase<T>
    {
        protected Bitmap input;
        protected ITiledImage<T> output;
        protected ISubstitutionTable<T> table;

        public virtual ISubstitutionTable<T> SubstitutionTable
        {
            get { return table; }

            set { table = value; }
        }

        public virtual Bitmap InputImage
        {
            get { return input; }

            set { input = value; }
        }

        public virtual int TileSize { get; set; }

        public virtual ITiledImage<T> Result
        {
            get { return output; }
        }

        public abstract ITiledImage<T> Pixelate();
    }
}