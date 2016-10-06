using System.Drawing;

namespace Twipe.Core.Internals
{
    public interface IPixelIdentifier
    {
        Bitmap Image { set; }

        int TotalPixels { get; }

        int WhitePixels { get; }

        int BlackPixels { get; }

        double WhiteRatio { get; }

        double BlackRatio { get; }

        double WhiteToBlackRatio { get; }

        double BlackToWhiteRatio { get; }

        void IdentifyPixels();
    }
}