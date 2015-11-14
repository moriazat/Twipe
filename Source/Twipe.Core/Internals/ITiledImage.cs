using System;
using System.Drawing;

namespace Twipe.Core.Internals
{
    /// <summary>
    /// Provides the interface for classes that represent result of a pixelation process.
    /// </summary>
    /// <typeparam name="T">Type of the object used in each tile.</typeparam>
    public interface ITiledImage<T> : IDisposable
    {
        int Rows { get; }

        int Columns { get; }

        int Height { get; }

        int Width { get; }

        int TileSize { get; }

        Image Image { get; set; }

        void SetTile(int x, int y, T tile);

        T GetTile(int x, int y);
    }
}