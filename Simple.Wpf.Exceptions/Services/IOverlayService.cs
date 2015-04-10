using System;
using Simple.Wpf.Exceptions.ViewModels;

namespace Simple.Wpf.Exceptions.Services
{
    public interface IOverlayService
    {
        IObservable<OverlayViewModel> Show { get; }

        void Post(string header, BaseViewModel viewModel, IDisposable lifetime);
    }
}