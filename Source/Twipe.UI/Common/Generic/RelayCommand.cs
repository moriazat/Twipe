using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Twipe.UI.Common.Generic
{
    public class RelayCommand<T> : RelayCommandBase<Action<T>, Func<T, bool>>
    {
        public RelayCommand(Action<T> executeMethod) : base(executeMethod)
        {
            // do nothing!
        }

        public RelayCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod) 
            : base(executeMethod, canExecuteMethod)
        {
            // do nothing!
        }

        public override bool CanExecute(object parameter)
        {
            if (canExecuteMethod != null)
            {
                T arg = (T)parameter;
                return  canExecuteMethod(arg);
            }

            return HasMethodToExecute();
        }

        public override void Execute(object parameter)
        {
            if (targetMethod != null)
            {
                T arg = (T)parameter;
                targetMethod(arg);
            }
        }
    }
}
