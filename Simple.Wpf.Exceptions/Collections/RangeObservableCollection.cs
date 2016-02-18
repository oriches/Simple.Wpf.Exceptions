using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Data;

namespace Simple.Wpf.Exceptions.Collections
{
    using Extensions;

    public sealed class RangeObservableCollection<T> : ObservableCollection<T>
    {
        private bool _suppressNotification;

        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotification)
            {
                var handlers = CollectionChanged;
                if (handlers != null)
                {
                    foreach (var handler in handlers.GetInvocationList().Cast<NotifyCollectionChangedEventHandler>())
                    {
                        if (handler.Target is CollectionView)
                        {
                            ((CollectionView)handler.Target).Refresh();
                        }
                        else
                        {
                            handler(this, e);
                        }
                    }
                }
            }
        }

        public void AddRange(IEnumerable<T> items)
        {
            _suppressNotification = true;

            var array = items.ToArray();
            array.ForEach(Add);

            _suppressNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, array, array.Length));
        }
    }
}
