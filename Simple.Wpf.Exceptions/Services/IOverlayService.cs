using System;
using Simple.Wpf.Exceptions.ViewModels;

namespace Simple.Wpf.Exceptions.Services
{
    public interface IOverlayService
    {
        IObservable<Message> Show { get; }

        void Post(string header, ClosableViewModel viewModel);
    }
}
