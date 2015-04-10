using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using NLog;
using Simple.Wpf.Exceptions.Commands;

namespace Simple.Wpf.Exceptions.ViewModels
{
    public abstract class CloseableViewModel : BaseViewModel, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Subject<Unit> _closing;
        private readonly Subject<Unit> _closed;
        private readonly IDisposable _disposable;

        protected CloseableViewModel()
        {
            _closing = new Subject<Unit>();
            _closed = new Subject<Unit>();

            CloseCommand = new RelayCommand(() =>
            {
                _closing.OnNext(Unit.Default);
                _closing.OnCompleted();

                _closed.OnNext(Unit.Default);
                _closed.OnCompleted();
            });

            _disposable = Disposable.Create(() =>
            {
                CloseCommand = null;

                _closing.Dispose();
                _closed.Dispose();
            });
        }

        public IObservable<Unit> Closing { get { return _closing; } }

        public IObservable<Unit> Closed { get { return _closed; } }

        public ICommand CloseCommand { get; private set; }

        public virtual void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose"))
            {
                _disposable.Dispose();
            }
        }

        protected void Close()
        {
            CloseCommand.Execute(null);
        }
    }
}