namespace Simple.Wpf.Exceptions.ViewModels
{
    using System;
    using System.Reactive;
    using System.Reactive.Subjects;
    using Commands;
    using Extensions;

    public abstract class CloseableViewModel : BaseViewModel, ICloseableViewModel
    {
        private readonly Subject<Unit> _closed;
        private readonly Subject<Unit> _denied;
        private readonly Subject<Unit> _confirmed;

        protected CloseableViewModel()
        {
            _closed = new Subject<Unit>()
                .DisposeWith(this);

            _denied = new Subject<Unit>()
               .DisposeWith(this);

            _confirmed = new Subject<Unit>()
               .DisposeWith(this);

            CancelCommand = ReactiveCommand.Create()
                .DisposeWith(this);

            CancelCommand.ActivateGestures()
                .Subscribe(x => _closed.OnNext(Unit.Default))
                .DisposeWith(this);

            InitialiseConfirmAndDeny();

            ConfirmCommand.ActivateGestures()
                .Subscribe(x =>
                {
                    _confirmed.OnNext(Unit.Default);
                    _closed.OnNext(Unit.Default);
                })
            .DisposeWith(this);

            DenyCommand.ActivateGestures()
                .Subscribe(x =>
                {
                    _denied.OnNext(Unit.Default);
                    _closed.OnNext(Unit.Default);
                })
            .DisposeWith(this);
        }

        public IObservable<Unit> Closed => _closed;
        public IObservable<Unit> Denied => _denied;
        public IObservable<Unit> Confirmed => _confirmed;
        public ReactiveCommand<object> CancelCommand { get; }
        public ReactiveCommand<object> ConfirmCommand { get; protected set; }
        public ReactiveCommand<object> DenyCommand { get; protected set; }

        protected abstract void InitialiseConfirmAndDeny();
    }
}
