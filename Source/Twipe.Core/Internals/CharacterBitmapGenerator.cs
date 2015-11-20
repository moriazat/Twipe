using System;
using System.Drawing;
using System.Drawing.Imaging;
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

        private unsafe byte* FirstPixelPointer { get; set; }

        public CharacterBitmapGenerator(ITiledImage<Character> input) : base(input)
        {
            charImages = new Bitmap[input.Columns, input.Rows];
        }

        public override async Task<Bitmap> GenerateImageAsync()
        {
            worker = Task.Factory.StartNew(GenerateImage);

            await worker;

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

                Bitmap image = null;

                //CreateImages(0);

                for (int row = 0; row < input.Rows; row++)
                {
                    //if (row + 1 < input.Rows)
                    //    CreateImagesAsync(row + 1);

                    for (int column = 0; column < input.Columns; column++)
                    {
                        //image = charImages[column, row];
                        using (image = GetImageFor(input.GetTile(column, row)))
                        {
                            int startingX = column * input.TileSize;
                            int startingY = row * input.TileSize;

                            DrawImage(image, startingX, startingY);

                            //image.Dispose();
                            //charImages[column] = null;
                        }
                    }
                }

                result.UnlockBits(resultData);
            }
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
            unsafe
            {
                int tileSize = input.TileSize;
                //Color pixel;
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
                        //pixel = image.GetPixel(x, y);

                        positionInBytes = (x * bytesPerPixel);

                        currentLine[positionInBytes] = *ptrPixelData;
                        currentLine[positionInBytes + 1] = *(ptrPixelData + 1);
                        currentLine[positionInBytes + 2] = *(ptrPixelData + 2);
                        currentLine[positionInBytes + 3] = *(ptrPixelData + 3);

                        ptrPixelData += 4;

                        //currentLine[positionInBytes] = (byte)pixel.B;
                        //currentLine[positionInBytes + 1] = (byte)pixel.G;
                        //currentLine[positionInBytes + 2] = (byte)pixel.R;
                        //currentLine[positionInBytes + 3] = (byte)pixel.A;
                    }
                }

                image.UnlockBits(tileData);
            }
        }
    }
}