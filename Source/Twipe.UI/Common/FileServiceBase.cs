
using Microsoft.Win32;

namespace Twipe.UI.Common
{
    public abstract class FileServiceBase : IFileService
    {
        protected string fileName;
        protected FileDialog dlg;

        public FileServiceBase(FileDialog dialog)
        {
            dlg = dialog;
        }

        public virtual string FileName
        {
            get { return fileName; }

            set { fileName = value; }
        }

        public abstract string SelectFile();
    }
}