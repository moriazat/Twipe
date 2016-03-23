using System.Drawing;
using System.Threading.Tasks;

namespace Twipe.Core.Internals
{
    public interface IBitmapConverter : IProgressable
    {
        Bitmap Result { get; }

        Task<Bitmap> ConvertAsync();
    }
}