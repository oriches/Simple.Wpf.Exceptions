using System;

namespace Simple.Wpf.Exceptions.Services
{
    using Models;
    using ViewModels;

    public interface IOverlayService
    {
        IObservable<Message> Show { get; }

        void Post(string header, CloseableViewModel viewModel);
    }
}
