namespace Codelet.Database
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using Codelet.Database.Entities;
  using Codelet.Domain;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// The database repository base class.
  /// </summary>
  /// <typeparam name="TDatabaseEntity">The type of the database entity.</typeparam>
  /// <typeparam name="TDatabaseEntityId">The type of the database entity identifier.</typeparam>
  /// <typeparam name="TModel">The type of the model.</typeparam>
  public abstract class DatabaseRepository<TDatabaseEntity, TDatabaseEntityId, TModel>
    where TDatabaseEntity : class, IDatabaseEntity, IDatabaseEntityWithId<TDatabaseEntityId>, IDatabaseEntityWithModel<TModel>
    where TModel : DomainModel
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseRepository{TModel, TDatabaseEntity, TDatabaseEntityId}" /> class.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="database" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger" /> == <c>null</c>.</exception>
    protected DatabaseRepository(
      DatabaseContext database,
      ILogger<DatabaseRepository<TDatabaseEntity, TDatabaseEntityId, TModel>> logger)
    {
      database = database ?? throw new ArgumentNullException(nameof(database));
      this.Entities = database.Set<TDatabaseEntity>();
      this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the entities.
    /// </summary>
    protected IDbSet<TDatabaseEntity> Entities { get; }

    /// <summary>
    /// Adds the specified model to the repository.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="model" /> == <c>null</c>.</exception>
    public virtual void Add(TModel model)
    {
      model = model ?? throw new ArgumentNullException(nameof(model));

      var entity = this.CreateEntity(model);
      entity.Synchronize(this.Logger);

      this.Entities.Add(entity);
    }

    /// <summary>
    /// Removes the specified model from the repository.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="model" /> == <c>null</c>.</exception>
    public virtual void Remove(TModel model)
    {
      model = model ?? throw new ArgumentNullException(nameof(model));

      var entity = this
        .Entities
        .FindEntityByModel(model)
        .OrThrow(() => new InvalidOperationException("Can't remove entity that is not in the database."));

      this.Entities.Remove(entity);
    }

    /// <summary>
    /// Finds the model by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The found model.</returns>
    public virtual async Task<Maybe<TModel>> FindByIdAsync(
      TDatabaseEntityId id,
      CancellationToken cancellationToken = default)
    {
      var entity = await this
        .Entities
        .FindEntityByIdAsync(id, cancellationToken)
        .ConfigureAwait(false);

      return entity?.Model;
    }

    /// <summary>
    /// Creates the entity.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>The created entity.</returns>
    protected abstract TDatabaseEntity CreateEntity(TModel model);
  }
}