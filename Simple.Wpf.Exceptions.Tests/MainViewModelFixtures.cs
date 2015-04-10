using System;
using System.Windows.Input;
using Moq;
using NUnit.Framework;
using Simple.Wpf.Exceptions.Extensions;
using Simple.Wpf.Exceptions.Services;
using Simple.Wpf.Exceptions.ViewModels;

namespace Simple.Wpf.Exceptions.Tests
{
    [TestFixture]
    public sealed class MainViewModelFixtures
    {
        private Mock<IGestureService> _gestureService;

        [SetUp]
        public void Setup()
        {
            _gestureService = new Mock<IGestureService>();
            _gestureService.Setup(x => x.SetBusy());
        }

        [Test]
        public void throws_exception_on_ui_thread()
        {
            // ARRANGE
            var exceptionMessage = "This is the exception message!";
            var viewModel = new MainViewModel(_gestureService.Object);

            // ACT
            Exception thrownException = null;
            try
            {
                viewModel.ThrowFromUiThreadCommand.Execute(exceptionMessage);
            }
            catch (Exception exn)
            {
                thrownException = exn;
            }

            // ASSERT
            Assert.That(thrownException, Is.Not.Null);
            Assert.That(thrownException.Message, Is.EqualTo(exceptionMessage));
        }

        [Test]
        public void disposing_clears_commands()
        {
            // ARRANGE
            var viewModel = new MainViewModel(_gestureService.Object);

            // ACT
            viewModel.Dispose();

            // ASSERT
            var commandProperties = TestHelper.PropertiesImplementingInterface<ICommand>(viewModel);
            commandProperties.ForEach(x => Assert.That(x.GetValue(viewModel, null), Is.Null));
        }
    }
}
