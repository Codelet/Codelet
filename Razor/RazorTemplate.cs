namespace Codelet.Razor
{
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Razor.Language;
  using Microsoft.CodeAnalysis.CSharp.Scripting;
  using Microsoft.CodeAnalysis.Scripting;

  /// <summary>
  /// Base class for Razor templates.
  /// </summary>
  public abstract class RazorTemplate : IRazorTemplate
  {
    /// <summary>
    /// Gets the script runner.
    /// </summary>
    protected abstract ScriptRunner<string> Script { get; }

    /// <inheritdoc />
    public Task<string> ExecuteAsync<TModel>(TModel model, CancellationToken cancellationToken = default)
      => this.Script(new Globals { Model = model }, cancellationToken);

    /// <summary>
    /// Compiles the specified razor source document.
    /// </summary>
    /// <param name="razorSourceDocument">The razor source document.</param>
    /// <returns>The compiled script runner.</returns>
    protected static ScriptRunner<string> Compile(RazorSourceDocument razorSourceDocument)
    {
      var razorCodeDocument = RazorCodeDocument.Create(razorSourceDocument);

      RazorEngine.Create().Process(razorCodeDocument);

      var csharp = razorCodeDocument.GetCSharpDocument();

      return CSharpScript.Create<string>(csharp.GeneratedCode).CreateDelegate();
    }

    private class Globals
    {
      public object Model { get; set; }

      public StringWriter Writer { get; } = new StringWriter();

      public void Write<TValue>(TValue value)
        => this.WriteLiteral(value.ToString());

      public void WriteLiteral(string value)
        => this.Writer.Write(value);
    }
  }
}