namespace Codelet.Domain
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Codelet.Testing.AutoFixture;
  using Moq;
  using Ploeh.AutoFixture.Xunit2;
  using Xunit;

  public class DomainEventHandlerTests
  {
    public class Constructor
    {
      [Theory, AutoMoqData]
      public DomainEventHandler<DomainModel, DomainEventArgs<DomainModel>> ConstructsInstance(
        IEnumerable<IDomainEventHandler<DomainModel, DomainEventArgs<DomainModel>>> eventHandlers,
        IEnumerable<IDomainModelScopeProvider<DomainModel>> modelScopeProviders,
        IEnumerable<IDomainEventScopeProvider<DomainModel, DomainEventArgs<DomainModel>>> eventScopeProviders)
        => new DomainEventHandler<DomainModel, DomainEventArgs<DomainModel>>(
          eventHandlers,
          modelScopeProviders,
          eventScopeProviders);

      [Theory, AutoMoqData]
      public void ThrowsIfEventHandlersIsNull(
        IEnumerable<IDomainModelScopeProvider<DomainModel>> modelScopeProviders,
        IEnumerable<IDomainEventScopeProvider<DomainModel, DomainEventArgs<DomainModel>>> eventScopeProviders)
        => Assert.Throws<ArgumentNullException>(
          "eventHandlers",
          () => new DomainEventHandler<DomainModel, DomainEventArgs<DomainModel>>(null, modelScopeProviders, eventScopeProviders));

      [Theory, AutoMoqData]
      public void ThrowsIfModelScopeProvidersIsNull(
        IEnumerable<IDomainEventHandler<DomainModel, DomainEventArgs<DomainModel>>> eventHandlers,
        IEnumerable<IDomainEventScopeProvider<DomainModel, DomainEventArgs<DomainModel>>> eventScopeProviders)
        => Assert.Throws<ArgumentNullException>(
          "modelScopeProviders",
          () => new DomainEventHandler<DomainModel, DomainEventArgs<DomainModel>>(eventHandlers, null, eventScopeProviders));

      [Theory, AutoMoqData]
      public void ThrowsIfEventScopeProvidersIsNull(
        IEnumerable<IDomainEventHandler<DomainModel, DomainEventArgs<DomainModel>>> eventHandlers,
        IEnumerable<IDomainModelScopeProvider<DomainModel>> modelScopeProviders)
        => Assert.Throws<ArgumentNullException>(
          "eventScopeProviders",
          () => new DomainEventHandler<DomainModel, DomainEventArgs<DomainModel>>(eventHandlers, modelScopeProviders, null));
    }

    public class HandleAsync
    {
      [Theory, AutoMoqData]
      public async Task ExecutesAllTheHandlers(
        [Frozen] IEnumerable<IDomainEventHandler<DomainModel, DomainEventArgs<DomainModel>>> handlers,
        DomainEventHandler<DomainModel, DomainEventArgs<DomainModel>> handler,
        DomainModel sender,
        DomainEventArgs<DomainModel> args,
        CancellationToken cancellationToken)
      {
        await handler.HandleAsync(sender, args, cancellationToken);

        foreach (var handlerMock in handlers)
        {
          Mock.Get(handlerMock).Verify(m => m.HandleAsync(sender, args, cancellationToken), Times.Once());
        }
      }

      [Theory, AutoMoqData]
      public async Task CreatesAllModelScopes(
        [Frozen] IEnumerable<IDomainModelScopeProvider<DomainModel>> handlers,
        DomainEventHandler<DomainModel, DomainEventArgs<DomainModel>> handler,
        DomainModel sender,
        DomainEventArgs<DomainModel> args,
        CancellationToken cancellationToken)
      {
        await handler.HandleAsync(sender, args, cancellationToken);

        foreach (var handlerMock in handlers)
        {
          Mock.Get(handlerMock).Verify(m => m.CreateScope(sender), Times.Once());
        }
      }

      [Theory, AutoMoqData]
      public async Task DisposesAllModelScopes(
        Mock<IDisposable> scope,
        [Frozen] IEnumerable<IDomainModelScopeProvider<DomainModel>> handlers,
        DomainEventHandler<DomainModel, DomainEventArgs<DomainModel>> handler,
        DomainModel sender,
        DomainEventArgs<DomainModel> args,
        CancellationToken cancellationToken)
      {
        foreach (var handlerMock in handlers)
        {
          Mock
            .Get(handlerMock)
            .Setup(m => m.CreateScope(sender))
            .Returns(scope.Object);
        }

        await handler.HandleAsync(sender, args, cancellationToken);

        scope.Verify(m => m.Dispose(), Times.Exactly(handlers.Count()));
      }

      [Theory, AutoMoqData]
      public async Task CreatesAllEventScopes(
        [Frozen] IEnumerable<IDomainEventScopeProvider<DomainModel, DomainEventArgs<DomainModel>>> handlers,
        DomainEventHandler<DomainModel, DomainEventArgs<DomainModel>> handler,
        DomainModel sender,
        DomainEventArgs<DomainModel> args,
        CancellationToken cancellationToken)
      {
        await handler.HandleAsync(sender, args, cancellationToken);

        foreach (var handlerMock in handlers)
        {
          Mock.Get(handlerMock).Verify(m => m.CreateScope(args), Times.Once());
        }
      }

      [Theory, AutoMoqData]
      public async Task DisposesAllEventScopes(
        Mock<IDisposable> scope,
        [Frozen] IEnumerable<IDomainEventScopeProvider<DomainModel, DomainEventArgs<DomainModel>>> handlers,
        DomainEventHandler<DomainModel, DomainEventArgs<DomainModel>> handler,
        DomainModel sender,
        DomainEventArgs<DomainModel> args,
        CancellationToken cancellationToken)
      {
        foreach (var handlerMock in handlers)
        {
          Mock
            .Get(handlerMock)
            .Setup(m => m.CreateScope(args))
            .Returns(scope.Object);
        }

        await handler.HandleAsync(sender, args, cancellationToken);

        scope.Verify(m => m.Dispose(), Times.Exactly(handlers.Count()));
      }
    }
  }
}