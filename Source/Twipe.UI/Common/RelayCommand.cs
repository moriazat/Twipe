using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Twipe.UI.Common
{
    public class RelayCommand : RelayCommandBase<Action, Func<bool>>
    {
        public RelayCommand(Action executeMethod) : base(executeMethod)
        {
            // do nothing!
        }

        public RelayCommand(Action executeMethod, Func<bool> canExecuteMethod) 
            : base(executeMethod, canExecuteMethod)
        {
            // do nothing!
        }

        public override bool CanExecute(object parameter)
        {
            if (canExecuteMethod != null)
                return canExecuteMethod();

            return HasMethodToExecute();
        }

        public override void Execute(object parameter)
        {
            if (targetMethod != null)
                targetMethod();    
        }


    }
}
