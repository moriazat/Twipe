
using Twipe.Core.Internals;

namespace Twipe.UI.Common
{
    public interface ISaveFileService : IFileService
    {
        void SaveFile<T>(ITiledImage<T> pixelatedImage);
    }
}