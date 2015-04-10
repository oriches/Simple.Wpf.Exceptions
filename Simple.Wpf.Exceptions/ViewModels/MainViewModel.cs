using System;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using NLog;
using Simple.Wpf.Exceptions.Commands;
using Simple.Wpf.Exceptions.Services;

namespace Simple.Wpf.Exceptions.ViewModels
{
    public sealed class MainViewModel : BaseViewModel, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDisposable _disposable;

        public MainViewModel(IGestureService gestureService)
        {
            ThrowFromUiThreadCommand = new RelayCommand<string>(x =>
            {
                gestureService.SetBusy();

                throw new Exception(x);
            });

            ThrowFromTaskCommand = new RelayCommand<string>(x =>
            {
                gestureService.SetBusy();

                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(100);

                    throw new Exception(x);
                }, TaskCreationOptions.LongRunning);
            });

            _disposable = Disposable.Create(() =>
            {
                ThrowFromUiThreadCommand = null;
                ThrowFromTaskCommand = null;
            });
        }

        public void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose"))
            {
                _disposable.Dispose();
            }
        }

        public ICommand ThrowFromUiThreadCommand { get; private set; }

        public ICommand ThrowFromTaskCommand { get; private set; }
    }
}
