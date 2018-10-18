namespace Codelet.Database
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using Codelet.Database.Entities;
  using Codelet.Domain;
  using Codelet.Functional;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// The database repository base class.
  /// </summary>
  /// <typeparam name="TDatabaseEntity">The type of the database entity.</typeparam>
  /// <typeparam name="TDatabaseEntityId">The type of the database entity identifier.</typeparam>
  /// <typeparam name="TModel">The type of the model.</typeparam>
  public abstract class DatabaseRepository<TDatabaseEntity, TDatabaseEntityId, TModel>
    : IRepository<TDatabaseEntityId, TModel>
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

    /// <inheritdoc />
    public virtual void Add(TModel model)
    {
      model = model ?? throw new ArgumentNullException(nameof(model));

      var entity = this.CreateEntity(model);
      entity.Synchronize(this.Logger);

      this.Entities.Add(entity);
    }

    /// <inheritdoc />
    public virtual void Remove(TModel model)
    {
      model = model ?? throw new ArgumentNullException(nameof(model));

      var entity = this
        .Entities
        .FindEntityByModel(model)
        .OrThrow(() => new InvalidOperationException("Can't remove entity that is not in the database."));

      this.Entities.Remove(entity);
    }

    /// <inheritdoc />
    public Maybe<TDatabaseEntityId> GetId(TModel model)
      => this.Entities.FindEntityByModel(model).Select(entity => entity.Id);

    /// <inheritdoc />
    public virtual async Task<Maybe<TModel>> FindByIdAsync(
      TDatabaseEntityId id,
      CancellationToken cancellationToken = default)
    {
      var entity = await this
        .Entities
        .FindEntityByIdAsync(id, cancellationToken)
        .ConfigureAwait(false);

      return entity.Select(e => e.Model);
    }

    /// <summary>
    /// Creates the entity.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>The created entity.</returns>
    protected abstract TDatabaseEntity CreateEntity(TModel model);
  }
}