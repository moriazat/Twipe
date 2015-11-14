using System.Threading.Tasks;

namespace Twipe.Core.Internals
{
    public interface ISubstitutionTableBuilder<T>
    {
        Task<ISubstitutionTable<T>> BuildAsync();
    }
}