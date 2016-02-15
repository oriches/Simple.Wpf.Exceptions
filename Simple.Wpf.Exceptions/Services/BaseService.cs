namespace Simple.Wpf.Exceptions.Services
{
    using System;
    using System.Reactive.Disposables;
    using NLog;

    public abstract class BaseService : IDisposable
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CompositeDisposable _disposable;

        protected BaseService()
        {
            _disposable = new CompositeDisposable();
        }

        public void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose - " + GetType().Name))
            {
                _disposable.Dispose();
            }
        }

        public void Add(IDisposable dispsoable)
        {
            _disposable.Add(dispsoable);
        }
    }
}