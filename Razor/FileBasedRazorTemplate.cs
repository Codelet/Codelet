namespace Codelet.Razor
{
  using System;
  using System.Collections.Concurrent;
  using System.IO;
  using Microsoft.AspNetCore.Razor.Language;
  using Microsoft.CodeAnalysis.Scripting;

  /// <summary>
  /// Razor template based on file source. Invalidates and re-generate itself whenever the source file is changed.
  /// </summary>
  public class FileBasedRazorTemplate : RazorTemplate
  {
    private static readonly ConcurrentDictionary<string, FileBasedRazorTemplate> Templates
      = new ConcurrentDictionary<string, FileBasedRazorTemplate>();

    private FileBasedRazorTemplate(string path)
    {
      this.Path = path;
      this.Watcher = new FileSystemWatcher(path);
      this.Watcher.Renamed += (_, args) => this.Path = args.FullPath;
      this.Watcher.Changed += (_, args) => this.ResetScript();
      this.ResetScript();
    }

    /// <summary>
    /// Gets the source file path.
    /// </summary>
    public string Path { get; private set; }

    /// <inheritdoc />
    protected override ScriptRunner<string> Script => this.ScriptFactory.Value;

    private FileSystemWatcher Watcher { get; }

    private Lazy<ScriptRunner<string>> ScriptFactory { get; set; }

    /// <summary>
    /// Creates the template from the specified <paramref name="file"/>.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <returns>The template.</returns>
    public static IRazorTemplate Create(FileInfo file)
      => Templates.GetOrAdd(
        file.FullName.ToLowerInvariant(),
        key => file.Exists
          ? new FileBasedRazorTemplate(key)
          : throw new InvalidOperationException($"File {file.FullName} must exist."));

    private void ResetScript()
      => this.ScriptFactory = new Lazy<ScriptRunner<string>>(this.Compile);

    private ScriptRunner<string> Compile()
    {
      using (var stream = File.OpenRead(this.Path))
      {
        var razorSourceDocument = RazorSourceDocument.ReadFrom(stream, System.IO.Path.GetFileName(this.Path));

        return Compile(razorSourceDocument);
      }
    }
  }
}