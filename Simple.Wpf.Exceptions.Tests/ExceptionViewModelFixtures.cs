namespace Simple.Wpf.Exceptions.Tests
{
    using System;
    using Moq;
    using NUnit.Framework;
    using Services;
    using ViewModels;

    [TestFixture]
    public sealed class ExceptionViewModelFixtures
    {
        private Mock<IApplicationService> _applicationService;

        [SetUp]
        public void Setup()
        {
            _applicationService = new Mock<IApplicationService>();

            var gestureService = new Mock<IGestureService>();
            gestureService.Setup(x => x.SetBusy());

            Extensions.ObservableExtensions.GestureService = gestureService.Object;
        }

        [Test]
        public void message_is_null_when_exception_is_null()
        {
            // ARRANGE
            // ACT
            var viewModel = new ExceptionViewModel(null, _applicationService.Object);

            // ASSERT
            Assert.That(viewModel.Message, Is.Null);
        }

        [Test]
        public void message_is_populated_when_exception_is_not_null()
        {
            // ARRANGE
            var message = "This is the message";
            var exception = new Exception(message);
            
            // ACT
            var viewModel = new ExceptionViewModel(exception, _applicationService.Object);

            // ASSERT
            Assert.That(viewModel.Message, Is.EqualTo(message));
        }

        [Test]
        public void can_not_copy_exception_to_clipboard_when_exception_is_null()
        {
            // ARRANGE
            var viewModel = new ExceptionViewModel(null, _applicationService.Object);

            // ACT
            var canExecute = viewModel.CopyCommand.CanExecute(null);

            // ASSERT
            Assert.That(canExecute, Is.False);
        }

        [Test]
        public void can_copy_exception_to_clipboard_when_exception_is_null()
        {
            // ARRANGE
            var message = "This is the message";
            var exception = new Exception(message);

            var viewModel = new ExceptionViewModel(exception, _applicationService.Object);

            // ACT
            var canExecute = viewModel.CopyCommand.CanExecute(null);
            
            // ASSERT
            Assert.That(canExecute, Is.True);
        }

        [Test]
        public void copy_exception_to_clipboard()
        {
            // ARRANGE
            var message = "This is the message";
            var exception = new Exception(message);

            _applicationService.Setup(x => x.CopyToClipboard(exception.ToString()));

            var viewModel = new ExceptionViewModel(exception, _applicationService.Object);

            // ACT
            viewModel.CopyCommand.Execute(null);
            
            // ASSERT
            _applicationService.Verify();
        }

        [Test]
        public void can_not_open_log_folder_when_there_is_no_log_folder()
        {
            // ARRANGE
            _applicationService.SetupGet<string>(x => x.LogFolder).Returns((string)null);

            var viewModel = new ExceptionViewModel(null, _applicationService.Object);

            // ACT
            var canExecute = viewModel.OpenLogFolderCommand.CanExecute(null);

            // ASSERT
            Assert.That(canExecute, Is.False);
        }

        [Test]
        public void can_open_log_folder_when_there_is_a_log_folder()
        {
            // ARRANGE
            _applicationService.SetupGet<string>(x => x.LogFolder).Returns(@"c:\temp\log.txt");

            var viewModel = new ExceptionViewModel(null, _applicationService.Object);

            // ACT
            var canExecute = viewModel.OpenLogFolderCommand.CanExecute(null);

            // ASSERT
            Assert.That(canExecute, Is.True);
        }

        [Test]
        public void opens_log_folder()
        {
            // ARRANGE
            _applicationService.SetupGet(x => x.LogFolder).Returns(@"c:\temp\log.txt");
            _applicationService.Setup(x => x.OpenFolder(@"c:\temp\log.txt"));

            var viewModel = new ExceptionViewModel(null, _applicationService.Object);

            // ACT
            viewModel.OpenLogFolderCommand.Execute(null);

            // ASSERT
            _applicationService.Verify();
        }

        [Test]
        public void exit_application()
        {
            // ARRANGE
            _applicationService.Setup(x => x.Exit());

            var viewModel = new ExceptionViewModel(null, _applicationService.Object);

            // ACT
            viewModel.ExitCommand.Execute(null);

            // ASSERT
            _applicationService.Verify();
        }

        [Test]
        public void restart_application()
        {
            // ARRANGE
            _applicationService.Setup(x => x.Exit());

            var viewModel = new ExceptionViewModel(null, _applicationService.Object);

            // ACT
            viewModel.RestartCommand.Execute(null);

            // ASSERT
            _applicationService.Verify();
        }

        [Test]
        public void continue_application()
        {
            // ARRANGE
            _applicationService.Setup(x => x.Exit());

            var viewModel = new ExceptionViewModel(null, _applicationService.Object);

            // ACT
            viewModel.ContinueCommand.Execute(null);

            // ASSERT
            _applicationService.Verify();
        }
    }
}
