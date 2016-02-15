namespace Simple.Wpf.Exceptions.Services
{
    using System;
    using ViewModels;

    public interface IOverlayService
    {
        IObservable<OverlayViewModel> Show { get; }

        void Post(string header, BaseViewModel viewModel, IDisposable lifetime);
    }
}