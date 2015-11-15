using Microsoft.Win32;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Twipe.Core.Internals;

namespace Twipe.UI.Common
{
    public class SaveFileService : FileServiceBase, ISaveFileService
    {
        public async void SaveFile<T>(ITiledImage<T> pixelatedImage)
        {
            await SaveFileAsync(pixelatedImage);
        }

        public override string SelectFile()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "JPEG File (*.jpg)|*.jpg|Bitmap File (*.bmp)|*.bmp|PNG File (*.png)|*.png";
            dlg.FileName = fileName;

            if ((bool)dlg.ShowDialog())
                fileName = dlg.FileName;

            return fileName;
        }

        private async Task SaveFileAsync<T>(ITiledImage<T> pixelatedImage)
        {
            CharacterBitmapGenerator generator =
                    new CharacterBitmapGenerator((ITiledImage<Character>)pixelatedImage);
            Bitmap result = await generator.GenerateImageAsync();
            result.Save(this.fileName);
            result.Dispose();
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
    }
}