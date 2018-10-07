namespace Codelet.Razor
{
  using Microsoft.AspNetCore.Razor.Language;
  using Microsoft.CodeAnalysis.Scripting;

  /// <summary>
  /// Razor template based on text source.
  /// </summary>
  /// <seealso cref="Codelet.Razor.RazorTemplate" />
  public class TextBasedRazorTemplate : RazorTemplate
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TextBasedRazorTemplate"/> class.
    /// </summary>
    /// <param name="source">The source.</param>
    public TextBasedRazorTemplate(string source)
    {
      this.Script = Compile(RazorSourceDocument.Create(source, "<dummy>"));
    }

    /// <inheritdoc />
    protected override ScriptRunner<string> Script { get; }
  }
}