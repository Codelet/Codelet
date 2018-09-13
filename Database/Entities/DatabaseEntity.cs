namespace Codelet.Database.Entities
{
  using System;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// The database entity base class.
  /// </summary>
  /// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
  public abstract class DatabaseEntity<TIdentifier> : IDatabaseEntity, IDatabaseEntityWithId<TIdentifier>
  {
    /// <summary>
    /// Reason to put in <see cref="ObsoleteAttribute"/> of default constructor of this and derived classes.
    /// </summary>
    protected const string ForEntityFrameworkOnlyObsoleteReason = "Default constructor is intended for usage by EF only.";

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseEntity{TIdentifier}"/> class.
    /// </summary>
    protected DatabaseEntity()
    {
    }

    /// <inheritdoc />
    public TIdentifier Id { get; set; }

    /// <inheritdoc />
    public abstract void Synchronize(ILogger logger);
  }
}