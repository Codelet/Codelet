namespace Codelet.Razor
{
  using System;
  using System.IO;
  using System.Threading.Tasks;

  /// <summary>
  /// Base class for compiled Razor template.
  /// </summary>
  public abstract class RazorTemplate
  {
    /// <summary>
    /// Gets the model.
    /// </summary>
    protected dynamic Model { get; private set; }

    private Action<string> WriteLiteralDelegate { get; set; }

    /// <summary>
    /// Executes template.
    /// </summary>
    /// <returns>The task that represents the process.</returns>
    public abstract Task ExecuteAsync();

    /// <summary>
    /// Executes the compiled template specified by <paramref name="templateType"/> against the given <paramref name="model"/>.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <param name="templateType">Type of the template.</param>
    /// <param name="model">The model.</param>
    /// <returns>The text rendered by template.</returns>
    internal static async Task<string> ExecuteAsync<TModel>(Type templateType, TModel model)
    {
      using (var writer = new StringWriter())
      {
        var template = (RazorTemplate)Activator.CreateInstance(templateType);
        template.Model = model;
        template.WriteLiteralDelegate = writer.Write;

        await template.ExecuteAsync().ConfigureAwait(false);

        return writer.ToString();
      }
    }

    /// <summary>
    /// Writes the literal.
    /// </summary>
    /// <param name="value">The value.</param>
    protected void WriteLiteral(string value)
    {
      this.WriteLiteralDelegate(value);
    }

    /// <summary>
    /// Writes the specified <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    protected void Write<TValue>(TValue value)
      => this.WriteLiteral(value.ToString());
  }
}