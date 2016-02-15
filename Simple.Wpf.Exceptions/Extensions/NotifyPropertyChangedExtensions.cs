namespace Simple.Wpf.Exceptions.Extensions
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reactive.Linq;

    public static class NotifyPropertyChangedExtensions
    {
        public static IObservable<PropertyChangedEventArgs> ObservePropertyChanged<TSource, TValue>(this TSource source,
            params Expression<Func<TSource, TValue>>[] properties)
            where TSource : INotifyPropertyChanged
        {
            var names = properties.Select(x => x.Body)
                .OfType<MemberExpression>()
                .Select(x => x.Member.Name);

            return source.ObservePropertyChanged()
                .Where(x => names.Contains(x.PropertyName));
        }

        public static IObservable<PropertyChangedEventArgs> ObservePropertyChanged(this INotifyPropertyChanged source)
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => source.PropertyChanged += h, h => source.PropertyChanged -= h)
                .Select(x => x.EventArgs);
        }
    }
}
