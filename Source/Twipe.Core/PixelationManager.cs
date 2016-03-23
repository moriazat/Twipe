using System;
using System.Threading.Tasks;
using Twipe.Core.Internals;

namespace Twipe.Core
{
    public class PixelationManager<T> : IProgressable, IDisposable
    {
        private ISubstitutionTableBuilder<T> tableBuilder;
        private PixelatorBase<T> pixelator;
        private IBitmapConverter converter;
        private ISubstitutionTable<T> table;
        private ITiledImage<T> result;
        private int tileSize;
        private float currentProgress;
        private float tableBuildingProgress;
        private float conversionProgress;
        private float pixelationProgress;
        private readonly float TableBuildingProgressFactor = 0.6F;
        private readonly float ConversionProgressFactor = 0.2F;
        private readonly float PixelationProgressFactor = 0.2F;

        public event EventHandler<ProgressEventArgs> ProgressChanged;

        public event EventHandler Completed;

        public ISubstitutionTableBuilder<T> SubstitutionTableBuilder
        {
            set
            {
                tableBuilder = value;
                tableBuilder.ProgressChanged += TableBuilder_ProgressChanged;
            }
        }

        public PixelatorBase<T> Pixelator
        {
            set
            {
                pixelator = value;
                pixelator.ProgressChanged += Pixelator_ProgressChanged;
            }
        }

        public IBitmapConverter Converter
        {
            set
            {
                converter = value;
                converter.ProgressChanged += Converter_ProgressChanged;
            }
        }

        public ITiledImage<T> Result
        {
            get { return result; }
        }

        public int TileSize
        {
            set { tileSize = value; }
        }

        public async Task<ITiledImage<T>> ProcessAsync()
        {
            Task tableTask = CreateTable();
            Task conversionTask = ConvertImage();

            await Task.WhenAll(tableTask, conversionTask);
            pixelator.InputImage = converter.Result;
            pixelator.SubstitutionTable = table;
            pixelator.TileSize = tileSize;
            result = await pixelator.PixelateAsync();

            OnCompleted();

            return result;
        }

        public void Dispose()
        {
            if (table != null)
                table.Dispose();

            if (result != null)
                result.Dispose();
        }

        private async Task CreateTable()
        {
            table = await tableBuilder.BuildAsync();
        }

        private async Task ConvertImage()
        {
            await converter.ConvertAsync();
        }

        private void OnProgressChanged()
        {
            currentProgress = tableBuildingProgress * TableBuildingProgressFactor;
            currentProgress += conversionProgress * ConversionProgressFactor;
            currentProgress += pixelationProgress * PixelationProgressFactor;

            if (ProgressChanged != null)
                ProgressChanged(this, new ProgressEventArgs(currentProgress, "Pixelating the image ..."));
        }

        private void OnCompleted()
        {
            if (Completed != null)
                Completed(this, new EventArgs());
        }

        private void TableBuilder_ProgressChanged(object sender, ProgressEventArgs e)
        {
            tableBuildingProgress = e.Progress;
            OnProgressChanged();
        }

        private void Converter_ProgressChanged(object sender, ProgressEventArgs e)
        {
            conversionProgress = e.Progress;
            OnProgressChanged();
        }

        private void Pixelator_ProgressChanged(object sender, ProgressEventArgs e)
        {
            pixelationProgress = e.Progress;
            OnProgressChanged();
        }
    }
}