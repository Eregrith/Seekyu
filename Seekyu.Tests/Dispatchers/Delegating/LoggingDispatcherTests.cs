using FluentAssertions;
using Moq;
using NUnit.Framework;
using Seekyu.Dispatchers.Delegating;
using Seekyu.Logging;
using System;

namespace Seekyu.Tests.Dispatchers.Delegating
{
    public class TestException : Exception
    {
        public TestException(string msg) : base(msg) { }
    }

    [TestFixture]
    class LoggingDispatcherTests
    {
        [Test]
        public void LoggingDispatcher_Should_Be_Derived_From_DelegatingQueryDispatcher()
        {
            typeof(LoggingDispatcher<IQuery>).Should().BeDerivedFrom<DelegatingDispatcher<IQuery>>();
        }

        [Test]
        public void Dispatch_Should_Call_Next_Dispatcher()
        {
            TestQuery testQuery = new TestQuery();
            Mock<IObjectSerializer> mockSerializer = new Mock<IObjectSerializer>();
            Mock<ILogger> mockLogger = new Mock<ILogger>();
            Mock<IDispatcher<IQuery>> mockDispatcher = new Mock<IDispatcher<IQuery>>();
            DelegatingDispatcher<IQuery> dispatcherTested = new LoggingDispatcher<IQuery>(mockSerializer.Object, mockLogger.Object);
            dispatcherTested.Next = mockDispatcher.Object;

            dispatcherTested.Dispatch<TestResponse>(testQuery);

            mockDispatcher.Verify(n => n.Dispatch<TestResponse>(testQuery), Times.Once);
        }

        [Test]
        public void Dispatch_Should_Log_Serialized_Query()
        {
            string expectedMessage = "Serialized Query";
            Mock<IObjectSerializer> mockSerializer = new Mock<IObjectSerializer>();
            Mock<ILogger> mockLogger = new Mock<ILogger>();
            Mock<IDispatcher<IQuery>> mockDispatcher = new Mock<IDispatcher<IQuery>>();
            DelegatingDispatcher<IQuery> dispatcherTested = new LoggingDispatcher<IQuery>(mockSerializer.Object, mockLogger.Object);
            dispatcherTested.Next = mockDispatcher.Object;
            TestQuery testQuery = new TestQuery();
            mockSerializer.Setup(m => m.Serialize(testQuery)).Returns(expectedMessage);

            dispatcherTested.Dispatch<object>(testQuery);

            mockSerializer.Verify(m => m.Serialize(testQuery), Times.Once);
            mockLogger.Verify(m => m.LogQuery(expectedMessage), Times.Once);
        }

        [Test]
        public void Dispatch_Should_Log_Serialized_Response()
        {
            string expectedMessage = "Serialized response";
            Mock<IObjectSerializer> mockSerializer = new Mock<IObjectSerializer>();
            Mock<ILogger> mockLogger = new Mock<ILogger>();
            Mock<IDispatcher<IQuery>> mockDispatcher = new Mock<IDispatcher<IQuery>>();
            DelegatingDispatcher<IQuery> dispatcherTested = new LoggingDispatcher<IQuery>(mockSerializer.Object, mockLogger.Object);
            dispatcherTested.Next = mockDispatcher.Object;
            TestQuery testQuery = new TestQuery();
            TestResponse testResponse = new TestResponse();
            mockDispatcher.Setup(m => m.Dispatch<object>(testQuery)).Returns(testResponse);
            mockSerializer.Setup(m => m.Serialize(testResponse)).Returns(expectedMessage);

            dispatcherTested.Dispatch<object>(testQuery);

            mockSerializer.Verify(m => m.Serialize(testResponse), Times.Once);
            mockLogger.Verify(m => m.LogResponse(expectedMessage), Times.Once);
        }

        [Test]
        public void Dispatch_Should_Log_Exception_And_Rethrow()
        {
            Mock<IObjectSerializer> mockSerializer = new Mock<IObjectSerializer>();
            Mock<ILogger> mockLogger = new Mock<ILogger>();
            Mock<IDispatcher<IQuery>> mockDispatcher = new Mock<IDispatcher<IQuery>>();
            DelegatingDispatcher<IQuery> dispatcherTested = new LoggingDispatcher<IQuery>(mockSerializer.Object, mockLogger.Object);
            dispatcherTested.Next = mockDispatcher.Object;
            TestQuery testQuery = new TestQuery();
            TestException exception = new TestException("boum");
            mockDispatcher.Setup(m => m.Dispatch<object>(testQuery)).Throws(exception);

            Assert.That(() => dispatcherTested.Dispatch<object>(testQuery), Throws.InstanceOf<TestException>());

            mockLogger.Verify(m => m.LogException(exception), Times.Once);
        }
    }
}
