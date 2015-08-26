using System;
using System.Reactive;
using Simple.Wpf.Exceptions.ViewModels;

namespace Simple.Wpf.Exceptions.Services
{
    public interface IMessageService
    {
        IObservable<MessageViewModel> Show { get; }

        IObservable<Unit> Post(string header, CloseableViewModel viewModel, IDisposable lifetime);
    }
}
