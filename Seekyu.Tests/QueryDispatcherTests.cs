using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;

namespace Seekyu.Tests
{
    [TestFixture]
    class QueryDispatcherTests
    {
        [Test]
        public void QueryDispatcher_Should_Implement_IQueryDispatcher()
        {
            typeof(QueryDispatcher).Should().Implement<IQueryDispatcher>();
        }

        [Test]
        public void Handle_Throws_MissingQueryHandlerException_When_No_Handler_Exists()
        {
            IQueryDispatcher dispatcherTested = new QueryDispatcher();

            Assert.That(() => dispatcherTested.Dispatch<TestResponse>(new TestQuery()),
                Throws.InstanceOf<MissingHandlerException>()
                    .With.Message.Contains("IHandler<TestQuery, TestResponse>"));
        }

        [Test]
        public void Handle_Should_Return_Response_From_Calling_TryHandle_Query_On_Existing_Handler()
        {
            TestQuery query = new TestQuery();
            TestResponse expectedResponse = new TestResponse();
            Mock<IQueryHandler<TestQuery, TestResponse>> mockHandler = new Mock<IQueryHandler<TestQuery, TestResponse>>();
            mockHandler.Setup(m => m.TryHandle(query)).Returns(expectedResponse);
            IQueryDispatcher dispatcherTested = new QueryDispatcher(mockHandler.Object);

            TestResponse response = dispatcherTested.Dispatch<TestResponse>(query);

            response.Should().BeSameAs(expectedResponse);
        }

        [Test]
        public void Handle_Should_Select_Correct_Handler_To_Dispatch_Query()
        {
            TestQuery query = new TestQuery();
            TestResponse expectedResponse = new TestResponse();
            Mock<IQueryHandler<TestQuery, TestResponse>> mockHandler = new Mock<IQueryHandler<TestQuery, TestResponse>>();
            mockHandler.Setup(m => m.TryHandle(query)).Returns(expectedResponse);
            Mock<IQueryHandler<TestQuery, OtherTestResponse>> otherMockHandler = new Mock<IQueryHandler<TestQuery, OtherTestResponse>>();
            otherMockHandler.Setup(m => m.TryHandle(query)).Throws(new InvalidOperationException("Wrong handler called"));
            IQueryDispatcher dispatcherTested = new QueryDispatcher(mockHandler.Object, otherMockHandler.Object);

            TestResponse response = dispatcherTested.Dispatch<TestResponse>(query);

            response.Should().BeSameAs(expectedResponse);
        }
    }
}
