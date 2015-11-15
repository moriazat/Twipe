using System;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Twipe.Core;
using Twipe.Core.Internals;
using Twipe.UI.Common;

namespace Twipe.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string tempFileName;
        private bool hasConverted;
        private bool canBrowse;
        private bool canSave;
        private BitmapImage originalImage;
        private BitmapImage convertedImage;
        private IOpenFileService openFileSrv;
        private ISaveFileService saveFileSrv;
        private CharacterConvesionSettingsViewModel cSettingVM;
        private PixelationManager<Character> manager;

        public MainWindowViewModel()
        {
            BrowseCommand = new RelayCommand(OnBrowse, CanBrowse);
            ConvertCommand = new RelayCommand(OnConvert, CanConvert);
            SaveCommand = new RelayCommand(OnSave, CanSave);
            openFileSrv = new OpenFileService();
            saveFileSrv = new SaveFileService();
            cSettingVM = new CharacterConvesionSettingsViewModel();
            hasConverted = false;
            canBrowse = true;
            canSave = false;
            tempFileName = System.IO.Path.GetTempPath() + "temp_image.jpg";
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
                EnableConversion();
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

        public RelayCommand BrowseCommand { get; private set; }

        public RelayCommand ConvertCommand { get; private set; }

        public RelayCommand SaveCommand { get; private set; }

        public CharacterConvesionSettingsViewModel CurrentSettingViewModel
        {
            get { return cSettingVM; }
        }

        private void OnBrowse()
        {
            string file = openFileSrv.SelectFile();

            if (!string.IsNullOrEmpty(file))
            {
                OriginalImage = new BitmapImage(new Uri(file));
                ConvertCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanBrowse()
        {
            return canBrowse;
        }

        private async void OnConvert()
        {
            DisableConversion();

            ConvertedImage = null;
            FreezeInteractions();

            manager = new PixelationManager<Character>();
            manager.SubstitutionTableBuilder = CurrentSettingViewModel.CreateTableBuilder();
            manager.TileSize = this.CurrentSettingViewModel.TileSize;
            manager.Pixelator = new Pixelator<Character>();
            manager.Converter = new AverageConverter(new Bitmap(originalImage.UriSource.LocalPath));
            await manager.ProcessAsync();

            CharacterBitmapGenerator generator = new CharacterBitmapGenerator(manager.Result);
            Bitmap image = await generator.GenerateImageAsync();
            image.Save(tempFileName);
            image.Dispose();

            ConvertedImage = new BitmapImage(new Uri(tempFileName));

            EnableConversion();
            ReleaseInteractions();
        }

        private void ReleaseInteractions()
        {
            canBrowse = true;
            canSave = true;
            BrowseCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
        }

        private void FreezeInteractions()
        {
            canBrowse = false;
            canSave = false;
            BrowseCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
        }

        private void DisableConversion()
        {
            hasConverted = true;
            ConvertCommand.RaiseCanExecuteChanged();
        }

        private void EnableConversion()
        {
            hasConverted = false;
            ConvertCommand.RaiseCanExecuteChanged();
        }

        private bool CanConvert()
        {
            if (!hasConverted && (OriginalImage != null))
                return true;

            return false;
        }

        private bool CanSave()
        {
            return canSave;
        }

        private void OnSave()
        {
            string destFileName = saveFileSrv.SelectFile();

            if (!string.IsNullOrEmpty(destFileName))
                saveFileSrv.SaveFile(manager.Result);
        }
    }
}