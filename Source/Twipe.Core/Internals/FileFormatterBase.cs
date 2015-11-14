using System.IO;

namespace Twipe.Core.Internals
{
    /// <summary>
    /// This is the base class for those classes that convert the provided ITiledImage object to
    /// a specific file format.
    /// </summary>
    /// <typeparam name="T">Type of the object that is used to fill the ITiledImage object with.</typeparam>
    public abstract class FileFormatterBase<T>
    {
        protected Stream stream;
        protected ITiledImage<T> image;

        public FileFormatterBase(ITiledImage<T> image, Stream stream)
        {
            this.stream = stream;
            this.image = image;
        }

        public Stream Stream
        {
            get { return this.stream; }
        }

        public abstract void WriteToStream();
    }
}