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
        private readonly Subject<Message> _show;
        
        public OverlayService()
        {
            _show = new Subject<Message>();

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

        public void Post(string header, CloseableViewModel viewModel)
        {
            _show.OnNext(new Message(header, viewModel));
        }

        public IObservable<Message> Show { get { return _show; } }
    }
}
