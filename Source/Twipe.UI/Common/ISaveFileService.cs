
using Twipe.Core.Internals;

namespace Twipe.UI.Common
{
    public interface ISaveFileService : IFileService, IProgressable
    {
        void SaveFileAs(string sourceFile);
    }
}