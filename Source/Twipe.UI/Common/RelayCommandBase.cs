using System;
using System.Windows.Input;

namespace Twipe.UI.Common
{
    public abstract class RelayCommandBase<TExecuteMethod, TCanExecuteFunc> : ICommand
    {
        protected TExecuteMethod targetMethod;
        protected TCanExecuteFunc canExecuteMethod;

        public event EventHandler CanExecuteChanged;

        public RelayCommandBase(TExecuteMethod executeMethod)
        {
            targetMethod = executeMethod;
        }

        public RelayCommandBase(TExecuteMethod executeMethod, TCanExecuteFunc canExecuteMethod)
        {
            targetMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        public abstract bool CanExecute(object parameter);

        public abstract void Execute(object parameter);

        public virtual void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        protected bool HasMethodToExecute()
        {
            if (targetMethod != null)
                return true;

            return false;
        }
    }
}
