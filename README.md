# Seekyu

## Purpose

This project is aimed at giving you a lightweight framework for dispatching and handling Command-pattern objects and get results in a generic automated way.

![Dispatching with Seekyu][dispatching_schema]

## Parts

It is comprised of two building blocks
  - Dispatcher
  - Handlers

The dispatcher is the central hub in which you register handlers for specific command/response pairs.
In practice, you would have one dispatcher and multiple handlers.

In details, there are two types of dispatchers :
  - Dispatcher<TDispatchable> : The base dispatcher which holds references to many handlers. Inherits from DecoratableDispatcher<TDispatchable>
  - DelegatingDispatcher<TDispatchable> : A middleware dispatcher which decorates any DecoratableDispatcher (through syntactic sugar, see below).

## Usage

### Handlers

You have to define your own handlers which have to implement this interface :

```csharp
namespace Seekyu
{
  public interface IHandler<TDispatchable, TResult> where TDispatchable : IDispatchable
}
```

e.g. :

```csharp
public interface IHitchickersQuery : IDispatchable { }

public class LifeUniverseAndEverythingQuery : IHitchickersQuery { }

public class GetAnswerToLifeHandler : IHandler<LifeUniverseAndEverythingQuery, int>
{
  public int TryHandle(LifeUniverseAndEverythingQuery query)
  {
    return 42;
  }
}
```

### Dispatcher

You have to instantiate a Dispatcher<TDispatchable> for each dispatchable category (= interface) you want to have dispatched. Common usage is separation of ICommand and IQuery in two dispatchers.
You give this Dispatcher instance an array of all the handlers you want it to browse when it needs to dispatch something.
No two handlers for the same dispatchable/response types pair can be registered.

```csharp
GetAnswerToLifeHandler deepThought = new GetAnswerToLifeHandler();
IHandlers[] handlers = new [] { deepThought };
Dispatcher<IHitchickersQuery> theGuide = new Dispatcher<IHitchickersQuery>(handlers);
```

And then you can dispatch your dispatchables:
```csharp
theGuide.Dispatch<int>(new LifeUniverseAndEverythingQuery()); //Returns 42;
```

[dispatching_schema]: https://i.imgur.com/GrNVuto.png "Dispatching with Seekyu"
