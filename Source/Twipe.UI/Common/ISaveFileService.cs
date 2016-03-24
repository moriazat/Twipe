
using Twipe.Core.Internals;

namespace Twipe.UI.Common
{
    public interface ISaveFileService : IFileService, IProgressable
    {
        void SaveFile<T>(ITiledImage<T> pixelatedImage);
    }
}