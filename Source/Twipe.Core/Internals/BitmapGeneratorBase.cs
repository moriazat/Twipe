using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Twipe.Core.Internals
{
    public abstract class BitmapGeneratorBase<T> : IBitmapGenerator<T>, IDisposable, IProgressable
    {
        protected ITiledImage<T> input;
        protected string resultName;
        protected Bitmap result;

        public event EventHandler<ProgressEventArgs> ProgressChanged;

        public event EventHandler Completed;

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

        protected virtual void OnProgressChanged(float progress)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, new ProgressEventArgs(progress, "Generating the result..."));
        }

        protected virtual void OnCompleted()
        {
            if (Completed != null)
                Completed(this, new EventArgs());
        }
    }
}