namespace Codelet.Domain
{
  using System;

  /// <summary>
  /// Domain event arguments marker class.
  /// </summary>
  /// <typeparam name="TDomainModel">The type of the domain model.</typeparam>
  public class DomainEventArgs<TDomainModel> : EventArgs
    where TDomainModel : DomainModel
  {
  }
}