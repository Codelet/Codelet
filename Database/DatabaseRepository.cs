namespace Codelet.Database
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Codelet.Database.Entities;
  using Codelet.Domain;
  using Codelet.Linq;
  using Microsoft.EntityFrameworkCore;

  /// <summary>
  /// The database repository base class.
  /// </summary>
  /// <typeparam name="TModel">The type of the model.</typeparam>
  /// <typeparam name="TDatabaseEntity">The type of the database entity.</typeparam>
  /// <typeparam name="TDatabaseEntityId">The type of the database entity identifier.</typeparam>
  public abstract class DatabaseRepository<TModel, TDatabaseEntity, TDatabaseEntityId>
    where TModel : DomainModel
    where TDatabaseEntity : class, IDatabaseEntity, IDatabaseEntityWithId<TDatabaseEntityId>, IDatabaseEntityWithModel<TModel>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseRepository{TModel, TDatabaseEntity, TDatabaseEntityId}" /> class.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="configuration">The configuration (can be <c>null</c>).</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="database" /> == <c>null</c>.</exception>
    protected DatabaseRepository(
      DbContext database,
      IDatabaseEntityConfiguration<TDatabaseEntity> configuration)
    {
      this.Database = database ?? throw new ArgumentNullException(nameof(database));
      this.DbSet = this.Database.Set<TDatabaseEntity>();
      this.Entities = configuration?.ConfigureIncludes(this.DbSet) ?? this.DbSet;
    }

    /// <summary>
    /// Gets the database.
    /// </summary>
    protected DbContext Database { get; }

    /// <summary>
    /// Gets the entities query.
    /// </summary>
    protected IQueryable<TDatabaseEntity> Entities { get; }

    /// <summary>
    /// Gets the local entities.
    /// </summary>
    protected IEnumerable<TDatabaseEntity> Local => this.DbSet.Local;

    private DbSet<TDatabaseEntity> DbSet { get; }

    /// <summary>
    /// Adds the specified model to the repository.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="model" /> == <c>null</c>.</exception>
    public virtual void Add(TModel model)
    {
      model = model ?? throw new ArgumentNullException(nameof(model));

      var entity = this.CreateEntity(model);
      entity.Synchronize();

      this.DbSet.Add(entity);
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
        .FindEntityByModel(model)
        .OrThrow(() => new InvalidOperationException("Can't remove entity that is not in the database."));

      this.DbSet.Remove(entity);
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
        .FindEntityByIdAsync(id, cancellationToken)
        .ConfigureAwait(false);

      return entity?.Model;
    }

    /// <summary>
    /// Finds the entity by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The found entity.</returns>
    protected Task<TDatabaseEntity> FindEntityByIdAsync(
      TDatabaseEntityId id,
      CancellationToken cancellationToken = default)
      => this.Entities.FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);

    /// <summary>
    /// Finds the entity by model.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>The found entity.</returns>
    protected Maybe<TDatabaseEntity> FindEntityByModel(TModel model)
      => this.DbSet.Local.FirstOrNone(entity => entity.Model == model);

    /// <summary>
    /// Creates the entity.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>The created entity.</returns>
    protected abstract TDatabaseEntity CreateEntity(TModel model);
  }
}