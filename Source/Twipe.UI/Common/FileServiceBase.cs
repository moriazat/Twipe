
namespace Twipe.UI.Common
{
    public abstract class FileServiceBase : IFileService
    {
        protected string fileName;

        public virtual string FileName
        {
            get { return fileName; }

            set { fileName = value; }
        }

        public abstract string SelectFile();
    }
}