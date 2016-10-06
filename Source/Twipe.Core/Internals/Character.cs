using System;
using System.Drawing;

namespace Twipe.Core.Internals
{
    /// <summary>
    /// Holds information about a single character, and how it is represented.
    /// </summary>
    public class Character : IDisposable
    {
        public Character()
        {
            // do nothing!
        }

        public Character(Font font, char characterValue)
        {
            Font = font;
            Value = characterValue;
        }

        public Font Font { get; set; }

        public char Value { get; set; }

        public void Dispose()
        {
            if (Font != null)
                Font.Dispose();
        }
    }
}