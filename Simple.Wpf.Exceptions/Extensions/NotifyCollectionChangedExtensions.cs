namespace Simple.Wpf.Exceptions.Extensions
{
    using System;
    using System.Collections.Specialized;
    using System.Reactive.Linq;

    public static class NotifyCollectionChangedExtensions
    {
        public static IObservable<NotifyCollectionChangedEventArgs> ObserveCollectionChanged(this INotifyCollectionChanged source)
        {
            return Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => source.CollectionChanged += h, h => source.CollectionChanged -= h)
                .Select(x => x.EventArgs);
        }
    }
}
