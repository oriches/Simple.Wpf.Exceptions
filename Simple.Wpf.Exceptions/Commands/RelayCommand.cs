using System;
using System.Windows.Input;

namespace Simple.Wpf.Exceptions.Commands
{
    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action execute)
            : base(x => execute())
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
            : base(x => execute(), x => canExecute())
        {
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute = x => true;

        public RelayCommand(Action<T> execute)
        {
            _execute = execute;
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public virtual void Execute(object parameter)
        {
            var typedParameter = parameter is T ? (T)parameter : default(T);

            if (CanExecute(typedParameter))
            {
                _execute(typedParameter);
            }
        }

        public virtual bool CanExecute(object parameter)
        {
            return _canExecute(parameter is T ? (T)parameter : default(T));
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

}
