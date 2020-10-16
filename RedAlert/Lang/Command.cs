using System;
using System.Windows.Input;

namespace RedAlert.Lang
{
    public class Command : ICommand
    {
        public delegate void Action<in T>(T optional = default);

        public delegate void Action();

        public event EventHandler CanExecuteChanged;        

        public RedAlert.Lang.Command.Action<object> ActionWithParam { get; set; }
        public RedAlert.Lang.Command.Action ActionNoParam { get; set; }

        public Command(Action action) => ActionNoParam = action;
        public Command(Action<object> action) => ActionWithParam = action;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ActionNoParam?.Invoke();
            ActionWithParam?.Invoke(parameter);
        }
    }
}
