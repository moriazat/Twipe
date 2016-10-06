using System.Threading.Tasks;

namespace Twipe.Core.Internals
{
    public interface ISubstitutionTableBuilder<T> : IProgressable
    {
        Task<ISubstitutionTable<T>> BuildAsync();
    }
}