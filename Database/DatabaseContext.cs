namespace Codelet.Database
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using System.Threading;
  using System.Threading.Tasks;
  using Codelet.Database.Entities;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// The database context.
  /// </summary>
  public abstract class DatabaseContext : DbContext
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseContext" /> class.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger" /> == <c>null</c>.</exception>
    protected DatabaseContext(DbContextOptions options, ILogger<DatabaseContext> logger)
      : base(options)
    {
      this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    protected ILogger Logger { get; }

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

    /// <inheritdoc />
    public override async Task<int> SaveChangesAsync(
      bool acceptAllChangesOnSuccess,
      CancellationToken cancellationToken = default)
    {
      foreach (var entity in this.Entities().ToArray())
      {
        entity.Synchronize(this.Logger);
      }

      return await base
        .SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken)
        .ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      var applyConfigurationGenericMethod = typeof(ModelBuilder).GetMethod(
        nameof(ModelBuilder.ApplyConfiguration),
        BindingFlags.Instance | BindingFlags.Public);

      var configurations = this.ConfigurationClasses.ToDictionary(
        configuration => configuration,
        configuration => configuration
          .GetInterfaces()
          .Where(IsConfiguration)
          .Select(type => type.GenericTypeArguments[0]));

      foreach (var configuration in configurations)
      {
        foreach (var databaseEntityType in configuration.Value)
        {
          // ReSharper disable once PossibleNullReferenceException
          applyConfigurationGenericMethod
            .MakeGenericMethod(databaseEntityType)
            .Invoke(modelBuilder, new[] { Activator.CreateInstance(configuration.Key) });
        }
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
      => type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>);
  }
}