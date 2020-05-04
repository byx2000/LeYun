using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LeYun.ViewModel
{
    class DelegateCommand : ICommand
    {
        //public event EventHandler CanExecuteChanged = delegate (object sender, EventArgs e) { };
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _Execute = execute;
            _CanExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_CanExecute == null)
            {
                return true;
            }
            return _CanExecute(parameter);
        }

        private readonly Func<object, bool> _CanExecute = null;

        public void Execute(object parameter)
        {
            _Execute(parameter);
        }

        private readonly Action<object> _Execute;
    }
}
