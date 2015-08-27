using System;
using System.Reactive.Linq;
using System.Windows.Input;
using NLog;
using Simple.Wpf.Exceptions.Models;
using Simple.Wpf.Exceptions.Services;

namespace Simple.Wpf.Exceptions.ViewModels
{
    public sealed class ChromeViewModel : BaseViewModel, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDisposable _disposable;

        private Message _message;
        
        public ChromeViewModel(MainViewModel main, IOverlayService overlayService)
        {
            Main = main;

            _disposable = overlayService.Show
                .Subscribe(UpdateOverlay);
        }
        
        public void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose"))
            {
                _disposable.Dispose();
            }
        }
        
        public MainViewModel Main { get; private set; }
        
        public ICommand CloseOverlayCommand { get { return _message != null ? _message.ViewModel.CloseCommand : null; } }

        public bool HasOverlay { get { return _message != null; } }

        public string OverlayHeader { get { return _message != null ? _message.Header : string.Empty; } }

        public CloseableViewModel Overlay { get { return _message != null ? _message.ViewModel : null; } }
        
        private void UpdateOverlay(Message message)
        {
            if (_message != null)
            {
                _message.ViewModel.Close();
            }

            if (message != null)
            {
                message.ViewModel.Closed
                    .Take(1)
                    .Subscribe(x => ClearOverlay());
            }

            _message = message;

            OnPropertyChanged(() => HasOverlay);
            OnPropertyChanged(() => Overlay);
            OnPropertyChanged(() => OverlayHeader);
            OnPropertyChanged(() => CloseOverlayCommand);
        }

        private void ClearOverlay()
        {
            _message = null;

            OnPropertyChanged(() => HasOverlay);
            OnPropertyChanged(() => Overlay);
            OnPropertyChanged(() => OverlayHeader);
            OnPropertyChanged(() => CloseOverlayCommand);
        }
    }
}
