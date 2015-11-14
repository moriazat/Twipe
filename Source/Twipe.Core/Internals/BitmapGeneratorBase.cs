using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Twipe.Core.Internals
{
    public abstract class BitmapGeneratorBase<T> : IBitmapGenerator<T>, IDisposable
    {
        protected ITiledImage<T> input;
        protected string resultName;
        protected Bitmap result;

        public BitmapGeneratorBase(ITiledImage<T> input)
        {
            this.input = input;
            result = new Bitmap(input.Width, input.Height);
        }

        public abstract Task<Bitmap> GenerateImageAsync();

        public virtual void Dispose()
        {
            if (result != null)
                result.Dispose();
        }
    }
}