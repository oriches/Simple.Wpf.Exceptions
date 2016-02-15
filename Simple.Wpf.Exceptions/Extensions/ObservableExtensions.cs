namespace Simple.Wpf.Exceptions.Extensions
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using Commands;
    using Services;

    public static class ObservableExtensions
    {
        public static IGestureService GestureService;

        public static IObservable<Unit> AsUnit<T>(this IObservable<T> observable)
        {
            return observable.Select(x => Unit.Default);
        }

        public static ReactiveCommand<object> ToCommand(this IObservable<bool> canExecute)
        {
            return ReactiveCommand.Create(canExecute);
        }

        public static IObservable<T> ActivateGestures<T>(this IObservable<T> observable)
        {
            if (GestureService == null)
            {
                throw new Exception("GestureService has not been initialised");
            }

            return observable.Do(x => GestureService.SetBusy());
        }
    }
}