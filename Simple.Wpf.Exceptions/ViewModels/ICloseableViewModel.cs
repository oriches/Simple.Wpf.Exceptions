namespace Simple.Wpf.Exceptions.ViewModels
{
    using System;
    using System.Reactive;

    public interface ICloseableViewModel : ITransientViewModel
    {
        IObservable<Unit> Closed { get; }
        IObservable<Unit> Denied { get; }
        IObservable<Unit> Confirmed { get; }
    }
}