using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace Twipe.Core.Internals
{
    public class Pixelator<T> : PixelatorBase<T>
    {
        private int totalPixels;

        public override async Task<ITiledImage<T>> PixelateAsync()
        {
            var worker = Task.Factory.StartNew<ITiledImage<T>>(Pixelate);

            return await worker;
        }

        public override ITiledImage<T> Pixelate()
        {
            totalPixels = input.Width * input.Height;
            int height = input.Height;
            int width = input.Width;
            output = new TiledImage<T>(input.Width, input.Height, TileSize);
            Color pixel;
            T tile;

            for (int x = 0; x < input.Width; x++)
            {
                for (int y = 0; y < input.Height; y++)
                {
                    pixel = input.GetPixel(x, y);
                    tile = table.GetSubstitutionFor(pixel.R);
                    Debug.Assert((tile != null), "Pixel substitution cannot be null.");
                    output.SetTile(x, y, tile);
                    OnProgressChanged((x * height) + y + 1);
                }
            }

            return output;
        }

        protected override void OnProgressChanged(float pixelCount)
        {
            float progress = pixelCount / (float)totalPixels;
            progress *= 100;

            base.OnProgressChanged(progress);
        }
    }
}