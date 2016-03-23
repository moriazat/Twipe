using System.Threading.Tasks;

namespace Twipe.Core.Internals
{
    public interface IRatioCalculator<T> : IProgressable
    {
        Task<SubstitutionItem<T>[]> ComputeRatiosAsync();
    }
}