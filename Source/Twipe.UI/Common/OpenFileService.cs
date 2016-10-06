using Microsoft.Win32;
using System;
using System.IO;

namespace Twipe.UI.Common
{
    public class OpenFileService : FileServiceBase, IOpenFileService
    {
        public OpenFileService(FileDialog dialog) : base(dialog)
        {
            dlg.Filter = "Image Files (*.jpg, *.jpeg, *.bmp, *.png)|*.jpg; *.jpeg; *.bmp; *.png";
        }

        public override string SelectFile()
        {
            if ((bool)dlg.ShowDialog())
                fileName = dlg.FileName;

            return fileName;
        }

        public Stream OpenFile()
        {
            throw new NotImplementedException();
        }
    }
}