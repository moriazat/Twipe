using System.Drawing;
using System.Threading.Tasks;

namespace Twipe.Core.Internals
{
    public abstract class BlackWhiteConverterBase : IBitmapConverter
    {
        protected Bitmap result;
        protected Bitmap input;

        public BlackWhiteConverterBase(Bitmap inputImage)
        {
            input = inputImage;
        }

        public virtual Bitmap Result
        {
            get { return result; }
        }

        public virtual async Task<Bitmap> ConvertAsync()
        {
            Task<Bitmap> convertionTask = Task.Factory.StartNew<Bitmap>(DoConvertion);

            Bitmap result = await convertionTask;

            result.Save(System.IO.Path.GetTempPath() + "\\temp_bw_image.jpg");

            return result;
        }

        protected virtual Bitmap DoConvertion()
        {
            result = new Bitmap(input.Width, input.Height);
            Color pixel;

            for (int x = 0; x < input.Width; x++)
            {
                for (int y = 0; y < input.Height; y++)
                {
                    pixel = input.GetPixel(x, y);
                    int newValue = GetAverageValue(pixel);
                    result.SetPixel(x, y, Color.FromArgb(newValue, newValue, newValue));
                }
            }

            return result;
        }

        protected abstract int GetAverageValue(Color pixel);
    }
}