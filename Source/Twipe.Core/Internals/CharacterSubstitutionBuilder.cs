using System;
using System.Drawing;
using System.Drawing.Text;
using System.Threading.Tasks;

namespace Twipe.Core.Internals
{
    /// <summary>
    /// Builds an instance of SubstitutionTable for Character objects.
    /// </summary>
    public class CharacterSubstitutionBuilder : ISubstitutionTableBuilder<Character>
    {
        private SubstitutionTable<Character> table;
        private ICharacterRatioCalculator calculator;
        private FontFamily defaultFontFamily;
        private FontSet fontSet;

        public event EventHandler<ProgressEventArgs> ProgressChanged;

        public event EventHandler Completed;

        public CharacterSubstitutionBuilder(ICharacterRatioCalculator calculator)
        {
            table = new SubstitutionTable<Character>();
            this.calculator = calculator;
            this.calculator.ProgressChanged += Calculator_ProgressChanged;
        }

        public int TileSize
        {
            set { calculator.TileSize = value; }
        }

        public FontFamily DefaultFontFamily
        {
            set { defaultFontFamily = value; }
        }

        public FontSet FontSet
        {
            set { fontSet = value; }
        }

        public ICharacterRatioCalculator Calculator
        {
            set { calculator = value; }
        }

        public async Task<ISubstitutionTable<Character>> BuildAsync()
        {
            FontFamily[] fontFamilies = GetFonts();
            calculator.FontFamilies = fontFamilies;
            SubstitutionItem<Character>[] substitutions = await calculator.ComputeRatiosAsync();

            foreach (SubstitutionItem<Character> s in substitutions)
                table.SetSubstitution(s);

            return table;
        }

        private FontFamily[] GetFonts()
        {
            FontFamily[] fonts = null;

            switch (fontSet)
            {
                case FontSet.Single:
                    fonts = new FontFamily[] { defaultFontFamily };
                    break;

                case FontSet.System:
                    fonts = GetSysetmFonts();
                    break;

                case FontSet.AllInstalled:
                    InstalledFontCollection fontCollection = new InstalledFontCollection();
                    fonts = fontCollection.Families;
                    break;
            }

            return fonts;
        }

        private FontFamily[] GetSysetmFonts()
        {
            return new FontFamily[]
            {
                SystemFonts.CaptionFont.FontFamily,
                SystemFonts.DefaultFont.FontFamily,
                SystemFonts.DialogFont.FontFamily,
                SystemFonts.IconTitleFont.FontFamily,
                SystemFonts.MenuFont.FontFamily,
                SystemFonts.MessageBoxFont.FontFamily,
                SystemFonts.SmallCaptionFont.FontFamily,
                SystemFonts.StatusFont.FontFamily
            };
        }

        private void OnProgressChanged(float progress, string message)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, new ProgressEventArgs(progress, message));
        }

        private void Calculator_ProgressChanged(object sender, ProgressEventArgs e)
        {
            OnProgressChanged(e.Progress, e.Message);
        }
    }
}