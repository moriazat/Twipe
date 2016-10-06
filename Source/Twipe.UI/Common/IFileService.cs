
namespace Twipe.UI.Common
{
    public interface IFileService
    {
        string SelectFile();

        string FileName { get; set; }
    }
}
