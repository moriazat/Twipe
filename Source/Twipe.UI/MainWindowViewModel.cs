using Twipe.Core;
using Twipe.UI.Common;
using System;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Twipe.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private BitmapImage originalImage;
        private BitmapImage convertedImage;
        private IOpenFileService openFileSrv;
        private CharacterConvetionSettingsViewModel cSettingVM;
        private PixelationManager<Character> manager;

        public MainWindowViewModel()
        {
            BrowseCommand = new RelayCommand(OnBrowse, CanBrowse);
            ConvertCommand = new RelayCommand(OnConvert, CanConvert);
            openFileSrv = new OpenFileService();
            cSettingVM = new CharacterConvetionSettingsViewModel();
        }

        public ImageSource OriginalImage
        {
            get
            {
                return originalImage;
            }
            set
            {
                originalImage = value as BitmapImage;
                OnPropertyChanged("OriginalImage");
            }
        }

        public ImageSource ConvertedImage
        {
            get
            {
                return convertedImage;
            }
            set
            {
                convertedImage = value as BitmapImage;
                OnPropertyChanged("ConvertedImage");
            }
        }
        
        public ICommand BrowseCommand { get; private set; }

        public ICommand ConvertCommand { get; private set; }

        public CharacterConvetionSettingsViewModel CurrentSettingViewModel
        {
            get { return cSettingVM; }
        }

        private void OnBrowse()
        {
            string file = openFileSrv.SelectFile();

            if (!string.IsNullOrEmpty(file))
                OriginalImage = new BitmapImage(new Uri(file));
        }

        private bool CanBrowse()
        {
            return true;
        }

        private async void OnConvert()
        {
            manager = new PixelationManager<Character>();
            manager.SubstitutionTableBuilder = CurrentSettingViewModel.CreateTableBuilder();
            manager.CellSzie = this.CurrentSettingViewModel.CellSize;
            manager.Pixelator = new Pixelator<Character>();
            manager.Converter = new AverageConverter(new Bitmap(originalImage.UriSource.LocalPath));
            await manager.ProcessAsync();

            CharacterBitmapGenerator generator = new CharacterBitmapGenerator(manager.Result);
            string fileName = System.IO.Path.GetTempPath() + "\\temp_image.jpg";
            Bitmap image = await generator.GenerateImageAsync();
            image.Save(fileName);
            image.Dispose();

            ConvertedImage = new BitmapImage(new Uri(fileName));

            manager.Result.Dispose();
            manager.Dispose();
        }

        private bool CanConvert()
        {
            //if (OriginalImage != null)
            //    return true;

            //return false;

            return true;
        }
    }
}
