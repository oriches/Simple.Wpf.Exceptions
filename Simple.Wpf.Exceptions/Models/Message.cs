using Simple.Wpf.Exceptions.ViewModels;

namespace Simple.Wpf.Exceptions.Models
{
    public sealed class Message
    {
        public Message(string header, CloseableViewModel viewModel)
        {
            Header = header;
            ViewModel = viewModel;
        }

        public string Header { get; private set; }

        public CloseableViewModel ViewModel { get; private set; }
    }
}
