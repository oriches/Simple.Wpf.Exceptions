namespace Simple.Wpf.Exceptions.Models
{
    using System;
    using System.Reactive.Disposables;
    using NLog;

    public abstract class DisposableObject : IDisposable
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CompositeDisposable _disposable;

        protected DisposableObject()
        {
            _disposable = new CompositeDisposable();
        }

        public virtual void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose - " + GetType().Name))
            {
                _disposable.Dispose();
            }
        }

        public static implicit operator CompositeDisposable(DisposableObject disposable)
        {
            return disposable._disposable;
        }
    }
}