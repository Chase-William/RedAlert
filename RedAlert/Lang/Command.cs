using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RedAlert.Lang
{
    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public Action ActionToBeExecuted { get; set; }

        public Command(Action action)
        {
            ActionToBeExecuted = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ActionToBeExecuted?.Invoke();
        }
    }
}
