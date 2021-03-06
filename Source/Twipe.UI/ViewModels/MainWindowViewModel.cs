﻿using System;
using System.Diagnostics;
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
        private float progress;
        private bool isProgressShown;
        private string statusMessage;
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
            openFileSrv = new OpenFileService(new Microsoft.Win32.OpenFileDialog());
            saveFileSrv = new SaveFileService(new Microsoft.Win32.SaveFileDialog());
            cSettingVM = new CharacterConvesionSettingsViewModel();
            hasConverted = false;
            canBrowse = true;
            canSave = false;
            tempFileName = System.IO.Path.GetTempPath() + "temp_image.png";
            statusMessage = "Ready";
            isProgressShown = false;
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

        public float Progress
        {
            get
            {
                return progress;
            }
            private set
            {
                this.progress = value;
                OnPropertyChanged("Progress");
            }
        }

        public string StatusMessage
        {
            get
            {
                return statusMessage;
            }
            set
            {
                statusMessage = value;
                OnPropertyChanged("StatusMessage");
            }
        }

        public bool IsProgressShown
        {
            get
            {
                return isProgressShown;
            }
            set
            {
                isProgressShown = value;
                OnPropertyChanged("IsProgressShown");
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
                StatusMessage = "Ready";
            }
        }

        private bool CanBrowse()
        {
            return canBrowse;
        }

        private async void OnConvert()
        {
            DisableConversion();
            IsProgressShown = true;

            ConvertedImage = null;
            FreezeInteractions();

            manager = new PixelationManager<Character>();
            manager.SubstitutionTableBuilder = CurrentSettingViewModel.CreateTableBuilder();
            manager.TileSize = this.CurrentSettingViewModel.TileSize;
            manager.Pixelator = new Pixelator<Character>();
            manager.Converter = new AverageConverter(new Bitmap(originalImage.UriSource.LocalPath));
            manager.ProgressChanged += ProgressChangedHandler;
            await manager.ProcessAsync();

            CharacterBitmapGenerator generator = new CharacterBitmapGenerator(manager.Result);
            generator.ProgressChanged += ProgressChangedHandler;
            generator.Completed += CompletedHandler;
            generator.UseTransparentTiles = CurrentSettingViewModel.UseTransparentTiles;
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
            {
                IsProgressShown = true;
                saveFileSrv.ProgressChanged += ProgressChangedHandler;
                saveFileSrv.Completed += CompletedHandler;
                saveFileSrv.SaveFileAs(tempFileName);
            }
        }

        private void ProgressChangedHandler(object sender, ProgressEventArgs e)
        {
            Progress = e.Progress;
            StatusMessage = e.Message;
        }

        private void CompletedHandler(object sender, EventArgs e)
        {
            Progress = 0;
            IsProgressShown = false;
            StatusMessage = "Ready";
        }
    }
}