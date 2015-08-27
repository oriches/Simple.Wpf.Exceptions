using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
        private readonly IOverlayService _overlayService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDisposable _disposable;
        private readonly SerialDisposable _overlayDisposable;

        public MainViewModel(IGestureService gestureService, IOverlayService overlayService, ISchedulerService schedulerService)
        {
            _overlayService = overlayService;

            ThrowFromUiThreadCommand = new RelayCommand<string>(x =>
            {
                Logger.Info("ThrowFromUiThreadCommand executing...");

                gestureService.SetBusy();

                throw new Exception(x + " - thrown from UI thread.");
            });

            ThrowFromTaskCommand = new RelayCommand<string>(x =>
            {
                Logger.Info("ThrowFromTaskCommand executing...");

                gestureService.SetBusy();

                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);

                    throw new Exception(x + " - thrown from Task StartNew.");
                }, TaskCreationOptions.LongRunning);
            });


            ThrowFromRxCommand = new RelayCommand<string>(x =>
            {
                Logger.Info("ThrowFromRxCommand executing...");

                gestureService.SetBusy();

                Observable.Start(() =>
                {
                    Thread.Sleep(1000);

                    throw new Exception(x + " - thrown from Rx Start.");
                }, schedulerService.TaskPool)
                .Take(1)
                .Subscribe();
            });

            OverlayCommand = new RelayCommand(Overlay);

            _overlayDisposable = new SerialDisposable();
            
            _disposable = new CompositeDisposable
            {
                _overlayDisposable,

                Disposable.Create(() =>
                {
                    ThrowFromUiThreadCommand = null;
                    ThrowFromTaskCommand = null;
                    ThrowFromRxCommand = null;
                    OverlayCommand = null;
                })
            };
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

        public ICommand ThrowFromRxCommand { get; private set; }

        public ICommand OverlayCommand { get; private set; }

        private void Overlay()
        {
            var viewModel = new OverlayViewModel();

            _overlayDisposable.Disposable = viewModel.Closed
                .Take(1)
                .Subscribe(x => viewModel.Dispose());

            _overlayService.Post("Overlay 1", viewModel);
        }
    }
}
