namespace Simple.Wpf.Exceptions.Commands
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Windows.Input;
    using NLog;

    public sealed class ReactiveCommand : ReactiveCommand<object>
    {
        private ReactiveCommand(IObservable<bool> canExecute)
            : base(canExecute.StartWith(false))
        {
        }

        public new static ReactiveCommand<object> Create()
        {
            return ReactiveCommand<object>.Create(Observable.Return(true).StartWith(true));
        }

        public new static ReactiveCommand<object> Create(IObservable<bool> canExecute)
        {
            return ReactiveCommand<object>.Create(canExecute);
        }
    }

    public class ReactiveCommand<T> : IObservable<T>, ICommand, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Subject<T> _execute;
        private readonly IDisposable _canDisposable;
        private bool _currentCanExecute;

        protected ReactiveCommand(IObservable<bool> canExecute)
        {
            _canDisposable = canExecute.Subscribe(x =>
            {
                _currentCanExecute = x;
                CommandManager.InvalidateRequerySuggested();
            });

            _execute = new Subject<T>();
        }

        public static ReactiveCommand<T> Create()
        {
            return new ReactiveCommand<T>(Observable.Return(true));
        }

        public static ReactiveCommand<T> Create(IObservable<bool> canExecute)
        {
            return new ReactiveCommand<T>(canExecute);
        }

        public void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose - " + GetType().Name))
            {
                _canDisposable.Dispose();

                _execute.OnCompleted();
                _execute.Dispose();
            }
        }

        public virtual void Execute(object parameter)
        {
            var typedParameter = parameter is T ? (T)parameter : default(T);

            if (CanExecute(typedParameter))
            {
                _execute.OnNext(typedParameter);
            }
        }

        public virtual bool CanExecute(object parameter)
        {
            return _currentCanExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _execute.Subscribe(observer.OnNext, observer.OnError, observer.OnCompleted);
        }
    }
}
