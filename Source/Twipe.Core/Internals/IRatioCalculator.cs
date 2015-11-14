using System.Threading.Tasks;

namespace Twipe.Core.Internals
{
    public interface IRatioCalculator<T>
    {
        Task<SubstitutionItem<T>[]> ComputeRatiosAsync();
    }
}