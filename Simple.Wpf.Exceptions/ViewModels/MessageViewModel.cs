using System;

namespace Simple.Wpf.Exceptions.ViewModels
{
    public sealed class MessageViewModel : OverlayViewModel<CloseableViewModel>
    {
        public MessageViewModel(string header, CloseableViewModel viewModel, IDisposable lifetime)
            : base(header, viewModel, lifetime)
        {
        }
    }
}