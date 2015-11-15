using System.IO;

namespace Twipe.UI.Common
{
    public interface IOpenFileService : IFileService
    {
        Stream OpenFile();
    }
}
