using FluentAssertions;
using NUnit.Framework;
using Seekyu.Dispatchers;
using Seekyu.Dispatchers.Delegating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seekyu.Tests.Dispatchers
{
    public class TestDecoratableDispatcher : DecoratableDispatcher<IQuery>
    {
        public override TResult Dispatch<TResult>(IQuery dispatchable)
        {
            throw new System.NotImplementedException();
        }
    }

    public class TestDelegatingDispatcher : DelegatingDispatcher<IQuery>
    {
        public override TResult Dispatch<TResult>(IQuery dispatchable)
        {
            throw new System.NotImplementedException();
        }
    }

    [TestFixture]
    class DecoratableDispatcherTests
    {
        [Test]
        public void DecoratableDispatcher_Should_Implement_IQueryDispatcher()
        {
            typeof(DecoratableDispatcher<IQuery>).Should().Implement<IDispatcher<IQuery>>();
        }

        [Test]
        public void DecorateWith_Should_Return_DelegatingDispatcher_With_Decoratable_Dispatcher_As_Next()
        {
            DelegatingDispatcher<IQuery> decorator = new TestDelegatingDispatcher();
            DecoratableDispatcher<IQuery> DecoratableDispatcher = new TestDecoratableDispatcher();

            DelegatingDispatcher<IQuery> result = DecoratableDispatcher.DecorateWith(decorator);

            result.Should().BeSameAs(decorator);
            result.Next.Should().Be(DecoratableDispatcher);
        }

        [Test]
        public void DecorateWith_Should_Be_Chainable()
        {
            DelegatingDispatcher<IQuery> decorator = new TestDelegatingDispatcher();
            DelegatingDispatcher<IQuery> decorator1 = new TestDelegatingDispatcher();
            DelegatingDispatcher<IQuery> decorator2 = new TestDelegatingDispatcher();
            DelegatingDispatcher<IQuery> decorator3 = new TestDelegatingDispatcher();
            DecoratableDispatcher<IQuery> DecoratableDispatcher = new TestDecoratableDispatcher();

            DelegatingDispatcher<IQuery> result = DecoratableDispatcher
                                                                    .DecorateWith(decorator)
                                                                    .DecorateWith(decorator1)
                                                                    .DecorateWith(decorator2)
                                                                    .DecorateWith(decorator3);

            result.Should().BeSameAs(decorator3);
            decorator3.Next.Should().Be(decorator2);
            decorator2.Next.Should().Be(decorator1);
            decorator1.Next.Should().Be(decorator);
            decorator.Next.Should().Be(DecoratableDispatcher);
        }
    }
}
