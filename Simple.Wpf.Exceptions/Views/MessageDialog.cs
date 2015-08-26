using System.Windows.Markup;
using MahApps.Metro.Controls.Dialogs;
using Simple.Wpf.Exceptions.ViewModels;

namespace Simple.Wpf.Exceptions.Views
{
    [ContentProperty("DialogBody")]
    public sealed class MessageDialog : BaseMetroDialog
    {
        private readonly MessageViewModel _viewModel;

        public MessageDialog(MessageViewModel viewModel)
        {
            _viewModel = viewModel;

            Title = _viewModel.Header;
            Content = _viewModel.ViewModel;
        }

        public CloseableViewModel CloseableContent { get { return _viewModel.ViewModel; } }
    }
}
