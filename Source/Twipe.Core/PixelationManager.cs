using System;
using System.Threading.Tasks;
using Twipe.Core.Internals;

namespace Twipe.Core
{
    public class PixelationManager<T> : IDisposable
    {
        private ISubstitutionTableBuilder<T> tableBuilder;
        private PixelatorBase<T> pixelator;
        private IBitmapConverter converter;
        private ISubstitutionTable<T> table;
        private ITiledImage<T> result;
        private int tileSize;

        public ISubstitutionTableBuilder<T> SubstitutionTableBuilder
        {
            set { tableBuilder = value; }
        }

        public PixelatorBase<T> Pixelator
        {
            set { pixelator = value; }
        }

        public IBitmapConverter Converter
        {
            set { converter = value; }
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
            Task convertionTask = ConvertImage();

            await Task.WhenAll(tableTask, convertionTask);
            pixelator.InputImage = converter.Result;
            pixelator.SubstitutionTable = table;
            pixelator.TileSize = tileSize;
            result = pixelator.Pixelate();

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
    }
}