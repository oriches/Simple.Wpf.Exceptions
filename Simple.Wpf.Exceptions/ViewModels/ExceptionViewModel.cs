using System;
using System.Reactive.Disposables;
using System.Windows.Input;
using NLog;
using Simple.Wpf.Exceptions.Commands;
using Simple.Wpf.Exceptions.Services;

namespace Simple.Wpf.Exceptions.ViewModels
{
    public sealed class ExceptionViewModel : CloseableViewModel
    {
        private readonly Exception _exception;
        private readonly IApplicationService _applicationService;
        private readonly IGestureService _gestureService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDisposable _disposable;

        public ExceptionViewModel(Exception exception, IApplicationService applicationService, IGestureService gestureService)
        {
            _exception = exception;
            _applicationService = applicationService;
            _gestureService = gestureService;

            CopyCommand = new RelayCommand(Copy, CanCopy);
            OpenLogFolderCommand = new RelayCommand(OpenLogFolder, CanOpenLogFolder);
            ContinueCommand = new RelayCommand(Close);
            ExitCommand = new RelayCommand(Exit);
            RestartCommand = new RelayCommand(Restart);

            _disposable = Disposable.Create(() =>
            {
                CopyCommand = null;
                OpenLogFolderCommand = null;
                ContinueCommand = null;
                ExitCommand = null;
                RestartCommand = null;
            });
        }

        public override void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose"))
            {
                base.Dispose();

                _disposable.Dispose();
            }
        }

        public ICommand CopyCommand { get; private set; }

        public ICommand OpenLogFolderCommand { get; private set; }

        public ICommand ContinueCommand { get; private set; }

        public ICommand ExitCommand { get; private set; }

        public ICommand RestartCommand { get; private set; }

        public string Message { get { return _exception.Message; } }

        private bool CanCopy()
        {
            return _exception != null;
        }

        private void Copy()
        {
            _gestureService.SetBusy();

            _applicationService.CopyToClipboard(_exception.ToString());
        }

        private bool CanOpenLogFolder()
        {
            return _applicationService.LogFolder != null;
        }

        private void OpenLogFolder()
        {
            _gestureService.SetBusy();

            _applicationService.OpenFolder(_applicationService.LogFolder);
        }

        private void Exit()
        {
            _applicationService.Exit();
        }

        private void Restart()
        {
            _applicationService.Restart();
        }
    }
}
