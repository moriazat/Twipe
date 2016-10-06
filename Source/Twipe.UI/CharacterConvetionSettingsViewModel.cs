using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Twipe.Core;
using System.Drawing;
using System.Drawing.Text;

namespace Twipe.UI.ViewModels
{
    public class CharacterConvetionSettingsViewModel : ViewModelBase
    {
        private FontSet fontSet;
        private FontFamily selectedFont;
        private bool canSelectFont;
        private int cellSize;

        public CharacterConvetionSettingsViewModel()
        {
            selectedFont = new FontFamily("Arial");
            fontSet = FontSet.AllInstalled;
            cellSize = 10;
        }

        public string[] FontSetList
        {
            get { return new string[] { "Single Font", "System Fonts", "All Installed Fonts" }; }
        }

        public string SelectedFontSet
        {
            get
            {
                return GetStringOf(fontSet);
            }
            set
            {
                fontSet = GetValueOf(value);
                if (fontSet == FontSet.Single)
                    CanSelectFont = true;
                else
                    CanSelectFont = false;

                OnPropertyChanged("SelectedFontSet");
            }
        }

        public int CellSize
        {
            get
            {
                return cellSize;
            }
            set
            {
                cellSize = value;
                OnPropertyChanged("CellSize");
            }
        }

        public string[] FontFamilyList
        {
            get { return GetFontsList(); }
        }

        public string SelectedFont
        {
            get
            {
                return selectedFont.Name;
            }
            set
            {
                selectedFont = new FontFamily(value);
                OnPropertyChanged("SelectedFont");
            }
        }

        public bool CanSelectFont
        {
            get
            {
                return canSelectFont;
            }
            set
            {
                canSelectFont = value;

                OnPropertyChanged("CanSelectFont");
            }
        }

        public ISubstitutionTableBuilder<Character> CreateTableBuilder()
        {
            CharactersRatioCaculator calc = new CharactersRatioCaculator(new AbsolutePixelIdentifier());
            CharacterSubstitutionBuilder builder = new CharacterSubstitutionBuilder(calc);
            builder.FontSet = fontSet;
            builder.CellSize = cellSize;
            builder.DefaultFontFamily = selectedFont;

            return builder;
        }

        private FontSet GetValueOf(string fontSet)
        {
            switch (fontSet)
            {
                case "Single Font":
                    return FontSet.Single;

                case "System Fonts":
                    return FontSet.System;

                case "All Installed Fonts":
                    return FontSet.AllInstalled;
            }

            throw new ArgumentException(string.Format("Invalid argument \"{0}\" supplied", fontSet));
        }

        private string GetStringOf(FontSet fontSet)
        {
            switch (fontSet)
            {
                case FontSet.Single:
                    return "Single Font";

                case FontSet.System:
                    return "System Fonts";

                case FontSet.AllInstalled:
                    return "All Installed Fonts";
            }

            throw new ArgumentException(string.Format("Invalid argument \"{0}\" supplied", fontSet));
        }

        private string[] GetFontsList()
        {
            InstalledFontCollection fonts = new InstalledFontCollection();
            FontFamily[] fontFamilies = fonts.Families;
            List<string> fontsNames = new List<string>();

            foreach (FontFamily f in fontFamilies)
                fontsNames.Add(f.Name);

            return fontsNames.ToArray();
        }
    }
}
