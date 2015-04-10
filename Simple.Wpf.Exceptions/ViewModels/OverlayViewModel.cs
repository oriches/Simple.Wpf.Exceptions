using System;

namespace Simple.Wpf.Exceptions.ViewModels
{
    public class OverlayViewModel<T>
    {
        public OverlayViewModel(string header, T viewModel, IDisposable lifetime)
        {
            Header = header;
            ViewModel = viewModel;
            Lifetime = lifetime;
        }

        public string Header { get; private set; }

        public T ViewModel { get; private set; }

        public IDisposable Lifetime { get; private set; }

        public bool HasLifetime { get { return Lifetime != null; } }
    }

    public sealed class OverlayViewModel : OverlayViewModel<BaseViewModel>
    {
        public OverlayViewModel(string header, BaseViewModel viewModel, IDisposable lifetime)
            : base(header, viewModel, lifetime)
        {
        }
    }
}