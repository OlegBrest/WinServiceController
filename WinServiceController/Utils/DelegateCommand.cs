using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WinServiceController.Utils
{
    public class DelegateCommand : ICommand
    {
        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        public Action CommandAction { get; set; }
        public Func<bool> CanExecuteFunc { get; set; }

        public DelegateCommand()
        {
        }

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute=null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteFunc != null) return CanExecuteFunc();
            else
            {
                return (this._canExecute == null || this._canExecute(parameter));
            }
        }

        public void Execute(object parameter)
        {
            if (CommandAction!=null) CommandAction();
            if (this._execute != null) this._execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
