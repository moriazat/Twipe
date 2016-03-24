
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Threading.Tasks;

namespace Twipe.Core.Internals
{
    public class CharacterBitmapGenerator : BitmapGeneratorBase<Character>
    {
        private Task worker;
        private BitmapData resultData;
        private int bytesPerPixel;
        private int heightInPixels;
        private int widthInBytes;
        private Bitmap[,] charImages;
        private StringBuilder sb;
        private Dictionary<int, Bitmap> tileCache;
        private Brush blackBrush;
        private Brush whiteBrush;
        private int tileSize;
        private int numberOfTotalTiles;
        private int numberOfTilesProcessed;

        private unsafe byte* FirstPixelPointer { get; set; }

        public CharacterBitmapGenerator(ITiledImage<Character> input) : base(input)
        {
            charImages = new Bitmap[input.Columns, input.Rows];
            numberOfTotalTiles = input.Columns * input.Rows;
            numberOfTilesProcessed = 0;
            sb = new StringBuilder();
            tileCache = new Dictionary<int, Bitmap>();
            blackBrush = Brushes.Black;
            whiteBrush = Brushes.White;
            tileSize = input.TileSize;
        }

        public override async Task<Bitmap> GenerateImageAsync()
        {
            worker = Task.Factory.StartNew(GenerateImage);

            await worker;

            OnCompleted();

            return result;
        }

        private void GenerateImage()
        {
            unsafe
            {
                resultData = result.LockBits(new Rectangle(0, 0, result.Width, result.Height), ImageLockMode.WriteOnly, result.PixelFormat);

                bytesPerPixel = Bitmap.GetPixelFormatSize(result.PixelFormat) / 8;
                heightInPixels = resultData.Height;
                widthInBytes = resultData.Width * bytesPerPixel;
                FirstPixelPointer = (byte*)resultData.Scan0;
                float progress;

                Bitmap image = null;

                for (int row = 0; row < input.Rows; row++)
                {
                    for (int column = 0; column < input.Columns; column++)
                    {
                        image = GetImageFor(input.GetTile(column, row));

                        int startingX = column * tileSize;
                        int startingY = row * tileSize;

                        DrawImage(image, startingX, startingY);

                        numberOfTilesProcessed++;
                    }

                    progress = (numberOfTilesProcessed / (float)numberOfTotalTiles) * 100;
                    OnProgressChanged(progress);
                }

                result.UnlockBits(resultData);

                CleanupCache();
            }
        }

        private unsafe void CleanupCache()
        {
            foreach (Bitmap b in tileCache.Values)
                b.Dispose();
        }

        private async void CreateImagesAsync(int row)
        {
            for (int column = 0; column < input.Columns; column++)
                charImages[column, row] = await GetImageForAsync(input.GetTile(column, row));
        }

        private void CreateImages(int row)
        {
            for (int column = 0; column < input.Columns; column++)
                charImages[column, row] = GetImageFor(input.GetTile(column, row));
        }

        private async Task<Bitmap> GetImageForAsync(Character c)
        {
            Task<Bitmap> task = Task.Run(() => GetImageFor(c));

            return await task;
        }

        private Bitmap GetImageFor(Character c)
        {
            int key = c.GetHashCode();
            if (tileCache.ContainsKey(key))
                return tileCache[key];

            Bitmap image = new Bitmap(tileSize, tileSize);

            using (Graphics g = Graphics.FromImage(image))
            {
                g.FillRectangle(whiteBrush, new Rectangle(0, 0, tileSize, tileSize));
                g.DrawString(c.Value.ToString(), c.Font, blackBrush, new Point(0, 0));
            }

            return image;
        }

        private string GenerateKey(Character c)
        {
            sb.Clear();
            sb.Append((int)c.Value);
            sb.Append("-");
            sb.Append(c.Font.Name);
            return sb.ToString();
        }

        private void DrawImage(Bitmap image, int startingX, int startingY)
        {
            unsafe
            {
                int positionInBytes;

                BitmapData tileData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                                                     ImageLockMode.ReadOnly, image.PixelFormat);
                byte* ptrPixelData = (byte*)tileData.Scan0;

                for (int y = 0; y < tileSize; y++)
                {
                    byte* currentLine = FirstPixelPointer + ((startingY + y) * resultData.Stride);
                    currentLine += startingX * bytesPerPixel;

                    for (int x = 0; x < tileSize; x++)
                    {
                        positionInBytes = (x * bytesPerPixel);

                        currentLine[positionInBytes] = *ptrPixelData;
                        currentLine[positionInBytes + 1] = *(ptrPixelData + 1);
                        currentLine[positionInBytes + 2] = *(ptrPixelData + 2);
                        currentLine[positionInBytes + 3] = *(ptrPixelData + 3);

                        ptrPixelData += 4;
                    }
                }

                image.UnlockBits(tileData);
            }
        }
    }
}