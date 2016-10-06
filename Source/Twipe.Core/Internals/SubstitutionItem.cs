using System;

namespace Twipe.Core.Internals
{
    /// <summary>
    /// Represents a substitutaion item used in SubstituationTable.
    /// </summary>
    /// <typeparam name="T">Type of object the item contains</typeparam>
    public class SubstitutionItem<T> : IDisposable
    {
        public SubstitutionItem()
        {
            // do nothing!
        }

        public SubstitutionItem(double grayRatio, T item)
        {
            GrayRatio = grayRatio;
            Value = item;
        }

        public double GrayRatio { get; set; }

        public T Value { get; set; }

        public void Dispose()
        {
            IDisposable garbage = Value as IDisposable;

            if (garbage != null)
                garbage.Dispose();
        }
    }
}