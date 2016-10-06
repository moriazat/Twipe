using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twipe.Core.Internals
{
    public interface IProgressable
    {
        event EventHandler<ProgressEventArgs> ProgressChanged;

        event EventHandler Completed;
    }
}