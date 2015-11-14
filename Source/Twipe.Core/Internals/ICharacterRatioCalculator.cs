using System.Drawing;

namespace Twipe.Core.Internals
{
    public interface ICharacterRatioCalculator : IRatioCalculator<Character>
    {
        FontFamily[] FontFamilies { set; }

        int TileSize { set; }
    }
}