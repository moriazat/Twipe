using System.Diagnostics;
using System.Drawing;

namespace Twipe.Core.Internals
{
    public class Pixelator<T> : PixelatorBase<T>
    {
        public override ITiledImage<T> Pixelate()
        {
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
                }
            }

            return output;
        }
    }
}