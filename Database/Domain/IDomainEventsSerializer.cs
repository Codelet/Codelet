namespace Codelet.Database.Domain
{
  using Codelet.Domain;

  /// <summary>
  /// Serializes the domain event into <see cref="string"/> to be stored in the database.
  /// </summary>
  public interface IDomainEventsSerializer
  {
    /// <summary>
    /// Serializes the specified arguments.
    /// </summary>
    /// <typeparam name="TDomainModel">The type of the domain model.</typeparam>
    /// <typeparam name="TDomainEventArgs">The type of the domain event arguments.</typeparam>
    /// <param name="args">The <typeparamref name="TDomainEventArgs"/> instance containing the event data.</param>
    /// <returns>The serialized string value.</returns>
    string Serialize<TDomainModel, TDomainEventArgs>(TDomainEventArgs args)
      where TDomainModel : DomainModel
      where TDomainEventArgs : DomainEventArgs<TDomainModel>;
  }
}