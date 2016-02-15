using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Simple.Wpf.Exceptions.Models;
using Simple.Wpf.Exceptions.Services;
using Simple.Wpf.Exceptions.ViewModels;

namespace Simple.Wpf.Exceptions.Tests
{
    using System.Reactive;
    using System.Reactive.Subjects;

    [TestFixture]
    public sealed class MessageServiceFixtures
    {
        [Test]
        public void posts_message_with_lifetime()
        {
            // ARRANGE
            var closed = new Subject<Unit>();
            var contentViewModel = new Mock<ICloseableViewModel>();
            contentViewModel.Setup(x => x.Closed).Returns(closed);

            var service = new MessageService();

            Message message = null;
            service.Show.Subscribe(x => message = x);

            // ACT
            service.Post("header 1", contentViewModel.Object);

            // ASSERT
            Assert.That(message.Header, Is.EqualTo("header 1"));
            Assert.That(message.ViewModel, Is.EqualTo(contentViewModel.Object));
        }

        [Test]
        public void posts_message_without_lifetime()
        {
            // ARRANGE
            var closed = new Subject<Unit>();
            var contentViewModel = new Mock<ICloseableViewModel>();
            contentViewModel.Setup(x => x.Closed).Returns(closed);

            var service = new MessageService();

            Message message = null;
            service.Show.Subscribe(x => message = x);

            // ACT
            service.Post("header 1", contentViewModel.Object);

            // ASSERT
            Assert.That(message.Header, Is.EqualTo("header 1"));
            Assert.That(message.ViewModel, Is.EqualTo(contentViewModel.Object));
        }

        [Test]
        public void posts_mulitple_messages()
        {
            // ARRANGE
            var closed1 = new Subject<Unit>();
            var contentViewModel1 = new Mock<ICloseableViewModel>();
            contentViewModel1.Setup(x => x.Closed).Returns(closed1);

            var closed2 = new Subject<Unit>();
            var contentViewModel2 = new Mock<ICloseableViewModel>();
            contentViewModel2.Setup(x => x.Closed).Returns(closed2);

            var service = new MessageService();

            var messages = new List<Message>();
            service.Show.Subscribe(x => messages.Add(x));

            service.Post("header 1", contentViewModel1.Object);
            service.Post("header 2", contentViewModel2.Object);

            // ACT
            closed1.OnNext(Unit.Default);

            // ASSERT
            Assert.That(messages.Count(x => x.Header == "header 1") == 1, Is.True);
            Assert.That(messages.Count(x => x.Header == "header 2") == 1, Is.True);
        }
    }
}
