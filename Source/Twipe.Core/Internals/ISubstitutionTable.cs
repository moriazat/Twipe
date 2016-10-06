using System;

namespace Twipe.Core.Internals
{
    /// <summary>
    /// Provides the interface for classes that substitute a single gray shade with an object of type T.
    /// </summary>
    /// <typeparam name="T">Type of the object used for substitution of a gray shade</typeparam>
    public interface ISubstitutionTable<T> : IDisposable
    {
        ShadeSubstitutionAcceptance AcceptanceRange { get; set; }

        T GetSubstitutionFor(int grayShadeValue);

        void SetSubstitution(double grayRatio, T item);

        void SetSubstitution(SubstitutionItem<T> substitutionItem);
    }
}