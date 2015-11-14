using System.Drawing;
using System.Threading.Tasks;

namespace Twipe.Core.Internals
{
    /// <summary>
    /// Provides interface for the classes that generate an image using a given ITiledImage object.
    /// </summary>
    public interface IBitmapGenerator<T>
    {
        Task<Bitmap> GenerateImageAsync();
    }
}