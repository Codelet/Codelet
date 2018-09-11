namespace Codelet.Database.Entities
{
  using System;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Metadata.Builders;

  /// <summary>
  /// Configuration for entities that have <see cref="Guid" /> as their database id.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  public abstract class GuidBasedDatabaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class, IDatabaseEntityWithId<Guid>
  {
    /// <inheritdoc />
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
      builder.Property(e => e.Id).ValueGeneratedNever();
    }
  }
}