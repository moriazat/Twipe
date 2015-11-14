using System.Drawing;
using System.Threading.Tasks;

namespace Twipe.Core.Internals
{
    public class CharacterBitmapGenerator : BitmapGeneratorBase<Character>
    {
        private Task worker;

        public CharacterBitmapGenerator(ITiledImage<Character> input) : base(input)
        {
            // do nothing!
        }

        public override async Task<Bitmap> GenerateImageAsync()
        {
            worker = Task.Factory.StartNew(GenerateImage);

            await worker;

            return result;
        }

        private void GenerateImage()
        {
            for (int row = 0; row < input.Rows; row++)
            {
                for (int column = 0; column < input.Columns; column++)
                {
                    using (Bitmap image = GetImageFor(input.GetTile(column, row)))
                    {
                        int startingX = column * input.TileSize;
                        int startingY = row * input.TileSize;

                        DrawImage(image, startingX, startingY);
                    }
                }
            }
        }

        private Bitmap GetImageFor(Character c)
        {
            Bitmap image = new Bitmap(input.TileSize, input.TileSize);

            using (Graphics g = Graphics.FromImage(image))
            {
                g.FillRectangle(Brushes.White, new Rectangle(0, 0, input.TileSize, input.TileSize));
                g.DrawString(c.Value.ToString(), c.Font, Brushes.Black, new Point(1, 1));
                g.Save();
            }

            return image;
        }

        private void DrawImage(Bitmap image, int startingX, int startingY)
        {
            int tileSize = input.TileSize;
            Color pixel;

            for (int x = 0; x < tileSize; x++)
            {
                for (int y = 0; y < tileSize; y++)
                {
                    pixel = image.GetPixel(x, y);
                    result.SetPixel(startingX + x, startingY + y, pixel);
                }
            }
        }
    }
}