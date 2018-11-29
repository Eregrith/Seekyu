using FluentAssertions;
using Moq;
using NUnit.Framework;
using Seekyu.Dispatchers;
using Seekyu.Exceptions;
using Seekyu.Logging;
using System;
using System.Collections.Generic;

namespace Seekyu.Tests
{
    public interface IQuery : IDispatchable { }

    public class TestQuery
        : IQuery
    {
        public DateTime Birthday;
        public string Name;
        public IList<int> Sizes;
        public TestQuery NextQuery;
        [LogEncrypted]
        public string Password { get; set; }
        [LogIgnored]
        public string LogContent { get; set; }
    }

    public class OtherTestQuery : IQuery { }

    public class TestResponse
    {
        public string Name { get; internal set; }
    }

    public class FakeHandler<TResult> : IHandler<TestQuery, TResult>
    {
        private readonly TResult FakeResponse;

        public FakeHandler(TResult fakeResponse)
        {
            FakeResponse = fakeResponse;
        }

        public TResult TryHandle(TestQuery dispatchable)
        {
            return FakeResponse;
        }
    }

    public class MultiInterfaceHandler
        : IHandler<TestQuery, int>,
          IHandler<OtherTestQuery, string>
    {
        public int TryHandle(TestQuery dispatchable)
        {
            return 42;
        }

        public string TryHandle(OtherTestQuery dispatchable)
        {
            return "42";
        }
    }

    [TestFixture]
    public class DispatcherTests
    {
        [Test]
        public void Dispatcher_Should_Be_Derived_From_DecoratableDispatcher()
        {
            typeof(Dispatcher<IQuery>).Should().BeDerivedFrom<DecoratableDispatcher<IQuery>>();
        }

        [Test]
        public void Dispatch_Should_Call_TryHandle()
        {
            TestResponse expectedResult = new TestResponse { Name = "Tata" };
            TestQuery testQuery = new TestQuery();
            FakeHandler<TestResponse> fakeHandler = new FakeHandler<TestResponse>(expectedResult);
            IDispatcher<IQuery> DispatcherTested = new Dispatcher<IQuery>(fakeHandler);

            TestResponse result = DispatcherTested.Dispatch<TestResponse>(testQuery);

            result.Should().BeSameAs(expectedResult);
        }

        [Test]
        public void Dispatch_Should_Call_TryHandle_On_Corresponding_Handler()
        {
            TestResponse expectedResult = new TestResponse { Name = "hjruihreigh" };
            FakeHandler<string> fakeStringHandler = new FakeHandler<string>("Fake");
            FakeHandler<TestResponse> fakeHandler = new FakeHandler<TestResponse>(expectedResult);
            IDispatcher<IQuery> DispatcherTested = new Dispatcher<IQuery>(fakeStringHandler, fakeHandler);
            TestQuery testQuery = new TestQuery();

            TestResponse response = DispatcherTested.Dispatch<TestResponse>(testQuery);

            response.Should().BeSameAs(expectedResult);
        }

        [Test]
        public void Dispatch_Should_Call_TryHandle_On_Corresponding_Handler_Interface()
        {
            MultiInterfaceHandler fakeHandler = new MultiInterfaceHandler();
            IDispatcher<IQuery> DispatcherTested = new Dispatcher<IQuery>(fakeHandler);
            TestQuery testQuery = new TestQuery();
            OtherTestQuery otherTestQuery = new OtherTestQuery();

            DispatcherTested.Dispatch<int>(testQuery).Should().Be(42);
            DispatcherTested.Dispatch<string>(otherTestQuery).Should().Be("42");
        }

        [Test]
        public void Dispatch_Should_Throw_MissingQueryHandlerException_When_No_Corresponding_Handler_Is_Found()
        {
            IDispatcher<IQuery> DispatcherTested = new Dispatcher<IQuery>();
            TestQuery testQuery = new TestQuery();
            string expectedMessage = $"No handler implementing IHandler<{testQuery.GetType().Name}, Object> has been found";

            Assert.That(() => DispatcherTested.Dispatch<object>(testQuery),
                Throws.TypeOf<MissingHandlerException>()
                    .With.Message.EqualTo(expectedMessage));
        }

        [Test]
        public void Constructor_Should_Throw_TooManyHandlersException_When_Too_Many_Handlers_Of_Same_Type()
        {
            IHandler[] handlers = new[]
            {
                new FakeHandler<string>("x"),
                new FakeHandler<string>("x")
            };

            Action a = () => new Dispatcher<IQuery>(handlers);

            a.Should().Throw<DuplicatedHandlerException>().WithMessage("There is already a Handler of <TestQuery, string> registered");
        }
    }
}
