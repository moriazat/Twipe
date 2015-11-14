using System;
using System.Drawing;

namespace Twipe.Core.Internals
{
    /// <summary>
    /// Represents the result of a pixelation process.
    /// </summary>
    /// <typeparam name="T">Type of object each tile contains</typeparam>
    public class TiledImage<T> : ITiledImage<T>
    {
        private int rows;
        private int columns;
        private int tileSize;
        private Image generatedImage;
        private T[,] tiles;

        public TiledImage(int width, int height, int tileSize)
        {
            rows = height;
            columns = width;
            this.tileSize = tileSize;
            tiles = new T[width, height];
        }

        public int TileSize
        {
            get { return tileSize; }
        }

        public int Rows
        {
            get { return rows; }
        }

        public int Height
        {
            get { return rows * tileSize; }
        }

        public Image Image
        {
            get { return generatedImage; }

            set { generatedImage = value; }
        }

        public int Columns
        {
            get { return columns; }
        }

        public int Width
        {
            get { return columns * tileSize; }
        }

        public T GetTile(int x, int y)
        {
            return tiles[x, y];
        }

        public void SetTile(int x, int y, T tile)
        {
            tiles[x, y] = tile;
        }

        public void Dispose()
        {
            IDisposable test = tiles[0, 0] as IDisposable;

            if (test != null)
                foreach (T c in tiles)
                    ((IDisposable)c).Dispose();
        }
    }
}