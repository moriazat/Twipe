using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Twipe.Core.Internals;

namespace Twipe.UI.Common
{
    public class SaveFileService : FileServiceBase, ISaveFileService, IProgressable
    {
        public event EventHandler Completed;

        public event EventHandler<ProgressEventArgs> ProgressChanged;

        public SaveFileService(FileDialog dialog) : base(dialog)
        {
            dlg.Filter = "JPEG File (*.jpg)|*.jpg|Bitmap File (*.bmp)|*.bmp|PNG File (*.png)|*.png";
        }

        public async void SaveFileAs(string sourceFile)
        {
            await Task.Run(() => SaveFileAsAsync(sourceFile));
        }

        public override string SelectFile()
        {
            dlg.FileName = string.Empty;

            if ((bool)dlg.ShowDialog())
                fileName = dlg.FileName;

            return fileName;
        }

        private void SaveFileAsAsync(string source)
        {
            // copy source file to a temp file to avoid conflict
            string tempFile = Path.GetTempPath() + "saveTempFile";
            File.Copy(source, tempFile, true);
            OnProgressChanged(20);

            Bitmap tempBitmap = new Bitmap(tempFile);
            OnProgressChanged(40);

            tempBitmap.Save(fileName);
            OnProgressChanged(60);

            tempBitmap.Dispose();
            OnProgressChanged(80);

            File.Delete(tempFile);
            OnProgressChanged(100);
            OnCompleted();
        }

        private void WriteToFile<T>(StreamWriter sw, ITiledImage<T> pixelatedImage)
        {
            for (int x = 0; x < pixelatedImage.Columns; x++)
            {
                for (int y = 0; y < pixelatedImage.Rows; y++)
                {
                    Character character = pixelatedImage.GetTile(x, y) as Character;
                    sw.Write(character.Value);
                }

                sw.WriteLine();
            }
        }

        private void OnCompleted()
        {
            if (Completed != null)
                Completed(this, new EventArgs());
        }

        private void OnProgressChanged(float progress)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, new ProgressEventArgs(progress, "Saving the result..."));
        }

        private void Generator_Completed(object sender, EventArgs e)
        {
            OnCompleted();
        }

        private void Generator_ProgressChanged(object sender, ProgressEventArgs e)
        {
            OnProgressChanged(e.Progress);
        }
    }
}