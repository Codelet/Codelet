namespace Codelet.Database.Entities
{
  using System;

  /// <summary>
  /// The database entity base class that contains a model.
  /// </summary>
  /// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
  /// <typeparam name="TModel">The type of the model.</typeparam>
  public abstract class DatabaseEntityWithModel<TIdentifier, TModel>
    : DatabaseEntity<TIdentifier>,
      IDatabaseEntityWithModel<TModel>
    where TModel : class
  {
    private readonly Lazy<TModel> model;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseEntityWithModel{TIdentifier, TModel}"/> class.
    /// </summary>
    /// <param name="model">The model.</param>
    protected DatabaseEntityWithModel(TModel model)
    {
      this.model = new Lazy<TModel>(() => model);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseEntityWithModel{TIdentifier, TModel}"/> class.
    /// </summary>
    [Obsolete(ForEntityFrameworkOnlyObsoleteReason)]
    protected DatabaseEntityWithModel()
    {
      this.model = new Lazy<TModel>(this.CreateModel);
    }

    /// <inheritdoc />
    public TModel Model => this.model.Value;

    /// <summary>
    /// Creates the model.
    /// </summary>
    /// <returns>The model.</returns>
    protected abstract TModel CreateModel();
  }
}