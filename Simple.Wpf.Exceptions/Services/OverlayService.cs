using System;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using NLog;
using Simple.Wpf.Exceptions.ViewModels;

namespace Simple.Wpf.Exceptions.Services
{
    public sealed class OverlayService : IOverlayService, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDisposable _disposable;
        private readonly Subject<OverlayViewModel> _show;
        
        public OverlayService()
        {
            _show = new Subject<OverlayViewModel>();

            _disposable = Disposable.Create(() =>
            {
                _show.OnCompleted();
                _show.Dispose();
            });
        }
        
        public void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose"))
            {
                _disposable.Dispose();
            }
        }

        public void Post(string header, BaseViewModel viewModel, IDisposable lifetime)
        {
            _show.OnNext(new OverlayViewModel(header, viewModel, lifetime));
        }

        public IObservable<OverlayViewModel> Show { get { return _show; } }
    }
}