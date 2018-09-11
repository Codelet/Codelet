namespace Codelet.Domain
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Codelet.Linq;

  /// <summary>
  /// Generic implementation of <see cref="IDomainEventHandler{DomainModel, TDomainEventArgs}"/>
  /// that gets all particular implementation of domain event handlers
  /// for the specified <typeparamref name="TDomainModel"/> and <typeparamref name="TDomainEventArgs"/>,
  /// surrounds them with corresponding <see cref="IDomainModelScopeProvider{TDomainModel}"/>
  /// and <see cref="IDomainEventScopeProvider{TDomainModel,TDomainEventArgs}"/> and executes all handlers.
  /// </summary>
  /// <typeparam name="TDomainModel">The type of the domain model.</typeparam>
  /// <typeparam name="TDomainEventArgs">The type of the domain event arguments.</typeparam>
  public class DomainEventHandler<TDomainModel, TDomainEventArgs>
    : IDomainEventHandler<TDomainModel, TDomainEventArgs>
    where TDomainModel : DomainModel
    where TDomainEventArgs : DomainEventArgs<TDomainModel>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventHandler{TDomainModel, TDomainEventArgs}"/> class.
    /// </summary>
    /// <param name="eventHandlers">The event handlers.</param>
    /// <param name="modelScopeProviders">The model scope providers.</param>
    /// <param name="eventScopeProviders">The event scope providers.</param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="eventHandlers" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="modelScopeProviders" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="eventScopeProviders" /> == <c>null</c>.</exception>
    public DomainEventHandler(
      IEnumerable<IDomainEventHandler<TDomainModel, TDomainEventArgs>> eventHandlers,
      IEnumerable<IDomainModelScopeProvider<TDomainModel>> modelScopeProviders,
      IEnumerable<IDomainEventScopeProvider<TDomainModel, TDomainEventArgs>> eventScopeProviders)
    {
      this.EventHandlers = eventHandlers ?? throw new ArgumentNullException(nameof(eventHandlers));
      this.ModelScopeProviders = modelScopeProviders ?? throw new ArgumentNullException(nameof(modelScopeProviders));
      this.EventScopeProviders = eventScopeProviders ?? throw new ArgumentNullException(nameof(eventScopeProviders));
    }

    private IEnumerable<IDomainEventHandler<TDomainModel, TDomainEventArgs>> EventHandlers { get; }

    private IEnumerable<IDomainModelScopeProvider<TDomainModel>> ModelScopeProviders { get; }

    private IEnumerable<IDomainEventScopeProvider<TDomainModel, TDomainEventArgs>> EventScopeProviders { get; }

    /// <inheritdoc />
    public async Task HandleAsync(
      TDomainModel sender,
      TDomainEventArgs args,
      CancellationToken сancellationToken)
    {
      using (this.ModelScopeProviders.Select(scopeProvider => scopeProvider.CreateScope(sender)).Merge())
      using (this.EventScopeProviders.Select(scopeProvider => scopeProvider.CreateScope(args)).Merge())
      {
        foreach (var domainEventHandler in this.EventHandlers)
        {
          await domainEventHandler
            .HandleAsync(sender, args, сancellationToken)
            .ConfigureAwait(false);
        }
      }
    }
  }
}