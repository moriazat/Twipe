using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace Twipe.Core.Internals
{
    public class CharactersRatioCaculator : ICharacterRatioCalculator
    {
        private FontFamily[] fontFamilies;
        private IPixelIdentifier algorithm;
        private int tileSize;
        private List<SubstitutionItem<Character>> ratios;
        private readonly string WidestCharacter = "W";

        public event EventHandler<ProgressEventArgs> ProgressChanged;

        public event EventHandler Completed;

        public CharactersRatioCaculator(IPixelIdentifier pixelIdentifier)
        {
            ratios = new List<SubstitutionItem<Character>>();
            algorithm = pixelIdentifier;
        }

        public FontFamily[] FontFamilies
        {
            set { fontFamilies = value; }
        }

        public int TileSize
        {
            set { tileSize = value; }
        }

        public async Task<SubstitutionItem<Character>[]> ComputeRatiosAsync()
        {
            var computationTask = Task.Factory.StartNew<SubstitutionItem<Character>[]>(ComputeRatios);

            var result = await computationTask;

            return result;
        }

        private SubstitutionItem<Character>[] ComputeRatios()
        {
            List<SubstitutionItem<Character>> ratiosList;
            int fontsCount = fontFamilies.Length;

            for (int i = 0; i < fontsCount; i++)
            {
                ratiosList = GetCharactersRatios(fontFamilies[i]);
                OnProgressChanged(i);
                AppendRatios(ratiosList);
            }

            var ratiosArray = ratios.ToArray();

            OnCompleted();

            return ratiosArray;
        }

        private void AppendRatios(List<SubstitutionItem<Character>> ratiosList)
        {
            int count = ratiosList.Count;

            for (int i = 0; i < count; i++)
                ratios.Add(ratiosList[i]);
        }

        private List<SubstitutionItem<Character>> GetCharactersRatios(FontFamily fontFamily)
        {
            float fontSize = CalculateFontSize(fontFamily);

            Font font = CreateFont(fontFamily, fontSize);

            Debug.Assert((font != null), "Font object cannot be null.");

            List<SubstitutionItem<Character>> list = new List<SubstitutionItem<Character>>();

            // iterate through visible characters
            for (int i = 32; i < 127; i++)
                list.Add(GetSubstitutionItem((byte)i, font));

            // iterate through extended ASCII characters
            for (int i = 128; i <= 255; i++)
                list.Add(GetSubstitutionItem((byte)i, font));

            return list;
        }

        private static Font CreateFont(FontFamily fontFamily, float fontSize)
        {
            if (fontFamily.IsStyleAvailable(FontStyle.Regular))
                return new Font(fontFamily, fontSize);
            else if (fontFamily.IsStyleAvailable(FontStyle.Bold))
                return new Font(fontFamily, fontSize, FontStyle.Bold);
            else if (fontFamily.IsStyleAvailable(FontStyle.Italic))
                return new Font(fontFamily, fontSize, FontStyle.Italic);
            else if (fontFamily.IsStyleAvailable(FontStyle.Strikeout))
                return new Font(fontFamily, fontSize, FontStyle.Strikeout);
            else if (fontFamily.IsStyleAvailable(FontStyle.Underline))
                return new Font(fontFamily, fontSize, FontStyle.Underline);

            return null;
        }

        private float CalculateFontSize(FontFamily fontFamily)
        {
            SizeF box;
            float fontSize = tileSize;

            Debug.Assert(tileSize != 0, "TileSize cannot be zero.");

            using (Bitmap temp = new Bitmap(tileSize, tileSize))
            {
                Graphics g = Graphics.FromImage(temp);
                //tr = new NativeTextRenderer(g);

                for (; fontSize > 0; fontSize--)
                {
                    using (Font font = CreateFont(fontFamily, fontSize))
                    {
                        box = g.MeasureString(WidestCharacter, font);

                        if (box.Height < tileSize && box.Width < tileSize)
                            break;
                    }
                }

                for (float f = 0.1F; f < 1; f += 0.1F)
                {
                    using (Font font = CreateFont(fontFamily, fontSize + f))
                    {
                        box = g.MeasureString(WidestCharacter, font);

                        if (box.Height >= tileSize || box.Width >= tileSize)
                        {
                            fontSize += (f - 0.1F);
                            break;
                        }
                    }
                }

                if (fontSize == 0)
                    fontSize = 0.1F;

                g.Dispose();
            }

            return fontSize;
        }

        private SubstitutionItem<Character> GetSubstitutionItem(byte characterCode, Font font)
        {
            char c = (char)characterCode;

            double ratio = GetRatio(c, font);
            return new SubstitutionItem<Character>(ratio, new Character(font, c));
        }

        private double GetRatio(char c, Font font)
        {
            Bitmap image = CreateCharacterImage(c, font);
            algorithm.Image = image;
            algorithm.IdentifyPixels();
            double ratio = algorithm.WhiteRatio;

            image.Dispose();

            return ratio;
        }

        private Bitmap CreateCharacterImage(char c, Font font)
        {
            Bitmap image = new Bitmap(tileSize, tileSize);

            using (Graphics g = Graphics.FromImage(image))
            {
                g.FillRectangle(Brushes.White, new Rectangle(0, 0, tileSize, tileSize));
                g.DrawString(c.ToString(), font, Brushes.Black, new Point(0, 0));
            }

            return image;
        }

        private void OnProgressChanged(int fontIndex)
        {
            if (ProgressChanged != null)
            {
                float progress = ((fontIndex + 1) / (float)fontFamilies.Length);
                progress *= 100;
                string message = string.Format("\"{0}\" font is added to the collection.", fontFamilies[fontIndex].Name);

                ProgressChanged(this, new ProgressEventArgs(progress, message));
            }
        }

        private void OnCompleted()
        {
            if (Completed != null)
                Completed(this, null);
        }
    }
}