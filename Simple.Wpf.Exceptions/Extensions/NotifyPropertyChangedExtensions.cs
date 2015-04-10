using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;

namespace Simple.Wpf.Exceptions.Extensions
{
    public static class NotifyPropertyChangedExtensions
    {
        public static IObservable<TValue> GetPropertyChangedValues<TSource, TValue>(this TSource source, Expression<Func<TSource, TValue>> property)
            where TSource : INotifyPropertyChanged
        {
            var memberExpression = property.Body as MemberExpression;
            var propertyName = memberExpression.Member.Name;
            var accessor = property.Compile();

            return source.GetPropertyChangedEvents()
                .Where(x => x.PropertyName == propertyName)
                .Select(x => accessor(source));
        }

        public static IObservable<PropertyChangedEventArgs>
            GetPropertyChangedEvents(this INotifyPropertyChanged source)
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => source.PropertyChanged += h, h => source.PropertyChanged -= h)
                .Select(x => x.EventArgs);
        }

        public static IObservable<PropertyChangedEventArgs>
            GetPropertyChangedEvents(this INotifyPropertyChanged source, string propertyName)
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => source.PropertyChanged += h, h => source.PropertyChanged -= h)
                .Select(x => x.EventArgs)
                .Where(x => x.PropertyName == propertyName);
        }

        public static IObservable<PropertyChangedEventArgs>
            GetPropertyChangedEvents<TSource, TValue>(this TSource source, Expression<Func<TSource, TValue>> property) where TSource : INotifyPropertyChanged
        {
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new Exception("Member expression is null!");
            }

            var propertyName = memberExpression.Member.Name;

            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => source.PropertyChanged += h, h => source.PropertyChanged -= h)
                .Select(x => x.EventArgs)
                .Where(x => x.PropertyName == propertyName);
        }
    }
}
