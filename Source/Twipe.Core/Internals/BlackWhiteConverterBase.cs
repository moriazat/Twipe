using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Twipe.Core.Internals
{
    public abstract class BlackWhiteConverterBase : IBitmapConverter
    {
        protected Bitmap result;
        protected Bitmap input;
        protected int totalPixels;

        public event EventHandler<ProgressEventArgs> ProgressChanged;

        public event EventHandler Completed;

        public BlackWhiteConverterBase(Bitmap inputImage)
        {
            input = inputImage;
            totalPixels = input.Width * input.Height;
        }

        public virtual Bitmap Result
        {
            get { return result; }
        }

        public virtual async Task<Bitmap> ConvertAsync()
        {
            Task<Bitmap> conversionTask = Task.Factory.StartNew<Bitmap>(DoConversion);

            Bitmap result = await conversionTask;

            return result;
        }

        protected virtual Bitmap DoConversion()
        {
            result = new Bitmap(input.Width, input.Height);

            unsafe
            {
                BitmapData inputData = input.LockBits(new Rectangle(0, 0, input.Width, input.Height), ImageLockMode.ReadOnly, input.PixelFormat);

                BitmapData resultData = result.LockBits(new Rectangle(0, 0, result.Width, result.Height), ImageLockMode.WriteOnly, input.PixelFormat);

                byte* inputPtr = (byte*)inputData.Scan0;
                byte* resultPtr = (byte*)resultData.Scan0;

                int bytesPerPixel = Bitmap.GetPixelFormatSize(input.PixelFormat) / 8;
                int average;

                average = ConvertPixels(ref inputPtr, ref resultPtr, bytesPerPixel);

                input.UnlockBits(inputData);
                result.UnlockBits(resultData);
            }

            return result;
        }

        private unsafe int ConvertPixels(ref byte* inputPtr, ref byte* resultPtr, int bytesPerPixel)
        {
            int average = 0;
            for (int i = 0; i < totalPixels; i++)
            {
                average = GetAverageValue(*(inputPtr + 2), *(inputPtr + 1), *inputPtr);
                *resultPtr = (byte)average;
                *(resultPtr + 1) = (byte)average;
                *(resultPtr + 2) = (byte)average;

                inputPtr += bytesPerPixel;
                resultPtr += bytesPerPixel;

                OnProgressChanged(i + 1);
            }

            return average;
        }

        protected abstract int GetAverageValue(Color pixel);

        protected abstract int GetAverageValue(int red, int green, int blue);

        protected void OnProgressChanged(int pixelCount)
        {
            float progress = pixelCount / (float)totalPixels;
            progress *= 100;

            if (ProgressChanged != null)
                ProgressChanged(this, new ProgressEventArgs(progress));
        }
    }
}