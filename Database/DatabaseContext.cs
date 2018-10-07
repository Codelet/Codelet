namespace Codelet.Database
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Codelet.Database.Entities;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.ChangeTracking;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// The database context.
  /// </summary>
  public abstract class DatabaseContext : IDisposable
  {
    private static readonly IReadOnlyCollection<Type> ConfigurationInterfaces = new[]
    {
      typeof(IEntityTypeConfiguration<>),
      typeof(IDatabaseEntityConfiguration<>),
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseContext" /> class.
    /// </summary>
    /// <param name="accessMode">The database access mode.</param>
    /// <param name="options">The options.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger" /> == <c>null</c>.</exception>
    protected DatabaseContext(
      DatabaseAccess accessMode,
      DbContextOptions options,
      ILogger<DatabaseContext> logger)
    {
      if (accessMode == DatabaseAccess.ReadOnly)
      {
        this.Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
      }

      var context = new EntityFrameworkDatabaseContext(options);
      context.ModelCreating += this.OnModelCreating;
      this.Context = context;

      this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Non generic database entity configuration to configure specific entity type (via generic implementation).
    /// </summary>
    private interface IDatabaseEntityConfigurator
    {
      /// <summary>
      /// Configures the database entity..
      /// </summary>
      /// <param name="modelBuilder">The model builder.</param>
      void Configure(ModelBuilder modelBuilder);
    }

    /// <summary>
    /// Gets the EF database context.
    /// </summary>
    protected DbContext Context { get; }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the entity classes.
    /// </summary>
    protected virtual IEnumerable<Type> EntityClasses => this
      .GetType()
      .Assembly
      .GetTypes()
      .Where(type => type.IsClass
        && !type.IsAbstract
        && !type.ContainsGenericParameters
        && type.GetInterfaces().Contains(typeof(IDatabaseEntity)));

    /// <summary>
    /// Gets the configuration classes.
    /// </summary>
    protected virtual IEnumerable<Type> ConfigurationClasses => this
      .GetType()
      .Assembly
      .GetTypes()
      .Where(type => type.IsClass
        && !type.IsAbstract
        && !type.ContainsGenericParameters
        && type.GetInterfaces().Any(IsConfiguration));

    /// <summary>
    /// Creates a <see cref="IDbSet{TDatabaseEntity}" /> that can be used to query and save instances of <typeparamref name="TDatabaseEntity" />.
    /// </summary>
    /// <typeparam name="TDatabaseEntity">The type of the database entity.</typeparam>
    /// <returns>A set for the given entity type.</returns>
    public IDbSet<TDatabaseEntity> Set<TDatabaseEntity>()
      where TDatabaseEntity : class
    {
      var configurationType = this
        .ConfigurationClasses
        .FirstOrDefault(typeof(IDatabaseEntityConfiguration<TDatabaseEntity>).IsAssignableFrom);

      var configuration = configurationType != null
        ? (IDatabaseEntityConfiguration<TDatabaseEntity>)Activator.CreateInstance(configurationType)
        : null;

      return new DbSetAdapter<TDatabaseEntity>(this.Context.Set<TDatabaseEntity>(), configuration);
    }

    /// <summary>
    /// Commits the database changes.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task that represent the process.</returns>
    public virtual async Task CommitAsync(CancellationToken cancellationToken = default)
    {
      if (this.Context.ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.NoTracking)
      {
        throw new InvalidOperationException("Database is in read only access mode. Commits are not allowed.");
      }

      foreach (var entity in this.Entities().ToArray())
      {
        entity.Synchronize(this.Logger);
      }

      await this.Context
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Dispose()
      => this.Context.Dispose();

    /// <inheritdoc cref='DbContext.OnModelCreating' />
    protected virtual void OnModelCreating(ModelBuilder modelBuilder)
    {
      foreach (var entityClass in this.EntityClasses)
      {
        var configurationInterface = typeof(IEntityTypeConfiguration<>).MakeGenericType(entityClass);

        var configurationType = this
          .ConfigurationClasses
          .FirstOrDefault(type => type.GetInterfaces().Contains(configurationInterface));

        var configuratorType = configurationType != null
          ? typeof(DatabaseEntityConfigurator<,>).MakeGenericType(entityClass, configurationType)
          : typeof(DatabaseEntityConfigurator<>).MakeGenericType(entityClass);

        var configurator = (IDatabaseEntityConfigurator)Activator.CreateInstance(configuratorType);
        configurator.Configure(modelBuilder);
      }
    }

    /// <summary>
    /// Get the entities of the specified <typeparamref name="TDatabaseEntity>"/> in the given <paramref name="states"/>.
    /// </summary>
    /// <remarks>
    /// Return all entities if <paramref name="states"/> has no values.
    /// </remarks>
    /// <typeparam name="TDatabaseEntity">The type of the database entity.</typeparam>
    /// <param name="states">The states.</param>
    /// <returns>The entities.</returns>
    protected IEnumerable<TDatabaseEntity> Entities<TDatabaseEntity>(params EntityState[] states)
      => this
        .Context
        .ChangeTracker
        .Entries()
        .Where(entry => states.Length == 0 || states.Contains(entry.State))
        .Select(entry => entry.Entity)
        .OfType<TDatabaseEntity>();

    /// <summary>
    /// Get the entities in the given <paramref name="states"/>.
    /// </summary>
    /// <remarks>
    /// Return all entities if <paramref name="states"/> has no values.
    /// </remarks>
    /// <param name="states">The states.</param>
    /// <returns>The entities.</returns>
    protected IEnumerable<IDatabaseEntity> Entities(params EntityState[] states)
      => this.Entities<IDatabaseEntity>();

    private static bool IsConfiguration(Type type)
      => type.IsConstructedGenericType && ConfigurationInterfaces.Contains(type.GetGenericTypeDefinition());

    private class EntityFrameworkDatabaseContext : DbContext
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="EntityFrameworkDatabaseContext" /> class.
      /// </summary>
      /// <param name="options">The options.</param>
      public EntityFrameworkDatabaseContext(DbContextOptions options)
        : base(options)
      {
      }

      /// <summary>
      /// Occurs when model is creating.
      /// </summary>
      public event Action<ModelBuilder> ModelCreating;

      /// <inheritdoc />
      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
        base.OnModelCreating(modelBuilder);

        this.ModelCreating?.Invoke(modelBuilder);
      }
    }

    private class DbSetAdapter<TDatabaseEntity> : IDbSet<TDatabaseEntity>
      where TDatabaseEntity : class
    {
      public DbSetAdapter(
        DbSet<TDatabaseEntity> entities,
        IDatabaseEntityConfiguration<TDatabaseEntity> configuration)
      {
        this.Entities = entities;
        this.Query = configuration?.ConfigureIncludes(this.Entities) ?? this.Entities;
        this.Local = new LocalViewAdapter<TDatabaseEntity>(this.Entities.Local);
      }

      public IQueryable<TDatabaseEntity> Query { get; }

      public IObservableReadOnlyCollection<TDatabaseEntity> Local { get; }

      private DbSet<TDatabaseEntity> Entities { get; }

      public void Add(TDatabaseEntity entity)
        => this.Entities.Add(entity);

      public void Remove(TDatabaseEntity entity)
        => this.Entities.Remove(entity);
    }

    private class LocalViewAdapter<TDatabaseEntity> : IObservableReadOnlyCollection<TDatabaseEntity>
      where TDatabaseEntity : class
    {
      public LocalViewAdapter(LocalView<TDatabaseEntity> source)
      {
        this.Source = source;
      }

      public event NotifyCollectionChangedEventHandler CollectionChanged
      {
        add => this.Source.CollectionChanged += value;
        remove => this.Source.CollectionChanged -= value;
      }

      public event PropertyChangedEventHandler PropertyChanged
      {
        add => this.Source.PropertyChanged += value;
        remove => this.Source.PropertyChanged -= value;
      }

      public event PropertyChangingEventHandler PropertyChanging
      {
        add => this.Source.PropertyChanging += value;
        remove => this.Source.PropertyChanging -= value;
      }

      public int Count => this.Source.Count;

      private LocalView<TDatabaseEntity> Source { get; }

      public IEnumerator<TDatabaseEntity> GetEnumerator()
        => this.Source.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator()
        => this.GetEnumerator();
    }

    private class DatabaseEntityConfigurator<TDatabaseEntity>
      : IDatabaseEntityConfigurator
      where TDatabaseEntity : class
    {
      /// <inheritdoc />
      public virtual void Configure(ModelBuilder modelBuilder)
      {
        modelBuilder.Entity<TDatabaseEntity>();
      }
    }

    private class DatabaseEntityConfigurator<TDatabaseEntity, TDatabaseEntityConfiguration>
      : DatabaseEntityConfigurator<TDatabaseEntity>
      where TDatabaseEntity : class
      where TDatabaseEntityConfiguration : class, IEntityTypeConfiguration<TDatabaseEntity>, new()
    {
      /// <inheritdoc />
      public override void Configure(ModelBuilder modelBuilder)
      {
        base.Configure(modelBuilder);

        modelBuilder.ApplyConfiguration(new TDatabaseEntityConfiguration());
      }
    }
  }
}