namespace Codelet.Razor
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Razor.Language;
  using Microsoft.AspNetCore.Razor.Language.Intermediate;
  using Microsoft.CodeAnalysis;
  using Microsoft.CodeAnalysis.CSharp;
  using Microsoft.CodeAnalysis.CSharp.Syntax;

  /// <summary>
  /// The factory methods to create Razor templates.
  /// </summary>
  public static class RazorTemplateFactory
  {
    private static readonly DirectiveDescriptor InheritsDirective
      = DirectiveDescriptor.CreateSingleLineDirective(
        "inherits",
        directive => directive.AddTypeToken());

    private static readonly RazorEngine Engine = RazorEngine.Create(ConfigureEngine);

    private static readonly ConcurrentDictionary<string, Type> StringTemplates
      = new ConcurrentDictionary<string, Type>();

    private static readonly ConcurrentDictionary<string, Type> FileTemplates
      = new ConcurrentDictionary<string, Type>();

    // ReSharper disable once CollectionNeverQueried.Local
    private static readonly IList<FileSystemWatcher> Watchers = new List<FileSystemWatcher>();

    /// <summary>
    /// Creates the Razor template from the given string <paramref name="source"/>.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <param name="key">The key.</param>
    /// <param name="source">The source.</param>
    /// <returns>The function that executes the template.</returns>
    public static Func<TModel, Task<string>> FromString<TModel>(string key, string source)
      => FromCache<TModel>(StringTemplates, key, _ => Compile(RazorSourceDocument.Create(source, "<dummy>")));

    /// <summary>
    /// Creates the Razor template from the given <paramref name="file"/>.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <param name="file">The file.</param>
    /// <returns>The function that executes the template.</returns>
    public static Func<TModel, Task<string>> FromFile<TModel>(FileInfo file)
      => FromCache<TModel>(FileTemplates, file.FullName.ToLowerInvariant(), RegisterFileTemplateSource);

    private static Func<TModel, Task<string>> FromCache<TModel>(
      ConcurrentDictionary<string, Type> cache,
      string key,
      Func<string, Type> factory)
      => model => RazorTemplate.ExecuteAsync(cache.GetOrAdd(key, factory), model);

    private static Type RegisterFileTemplateSource(string path)
    {
      Type CompileFile()
      {
        using (var stream = File.OpenRead(path))
        {
          var razorSourceDocument = RazorSourceDocument.ReadFrom(stream, path);

          return Compile(razorSourceDocument);
        }
      }

      if (!File.Exists(path))
      {
        throw new InvalidOperationException($"File {path} must exist.");
      }

      var watcher = new FileSystemWatcher(
        Path.GetDirectoryName(path) ?? throw new InvalidOperationException("Razor template file must contain folder."),
        Path.GetFileName(path));

      watcher.Renamed += (_, args) =>
      {
        FileTemplates[args.FullPath] = FileTemplates[args.OldFullPath];
        FileTemplates.TryRemove(args.OldFullPath, out var _);
      };

      watcher.Changed += (_, args) => FileTemplates[args.FullPath] = CompileFile();

      Watchers.Add(watcher);

      return CompileFile();
    }

    private static Type Compile(RazorSourceDocument razorSourceDocument)
    {
      var razorCodeDocument = RazorCodeDocument.Create(razorSourceDocument);
      Engine.Process(razorCodeDocument);

      var csharp = razorCodeDocument.GetCSharpDocument();

      var tree = CSharpSyntaxTree.ParseText(csharp.GeneratedCode);
      var compilation = CSharpCompilation
        .Create(Path.GetRandomFileName())
        .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
        .WithReferences(AppDomain
          .CurrentDomain
          .GetAssemblies()
          .Where(assembly => !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
          .Select(assembly => MetadataReference.CreateFromFile(assembly.Location)))
        .AddSyntaxTrees(tree);

      using (var stream = new MemoryStream())
      using (var pdbStream = new MemoryStream())
      {
        var result = compilation.Emit(stream, pdbStream);

        if (!result.Success)
        {
          throw new InvalidOperationException();
        }

        var assembly = Assembly.Load(stream.GetBuffer(), pdbStream.GetBuffer());

        var typeSyntax = tree
          .GetRoot()
          .DescendantNodes()
          .OfType<ClassDeclarationSyntax>()
          .First();

        var typeInfo = compilation.GetSemanticModel(tree, false).GetDeclaredSymbol(typeSyntax);
        var format = new SymbolDisplayFormat(
          typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

        return assembly.GetType(typeInfo.ToDisplayString(format));
      }
    }

    private static void ConfigureEngine(IRazorEngineBuilder builder)
    {
      var baseType = typeof(RazorTemplate);
      builder.SetNamespace(baseType.Namespace);
      builder.SetBaseType($"{baseType.Name}");
      builder.ConfigureClass((document, declaration) =>
      {
        declaration.ClassName = $"Template_{Guid.NewGuid().ToString().Replace('-', '_')}";
      });
      builder.AddDirective(InheritsDirective);
      builder.Features.Add(new InheritsPass());
    }

    private class InheritsPass : IntermediateNodePassBase, IRazorDirectiveClassifierPass
    {
      /// <inheritdoc />
      protected override void ExecuteCore(RazorCodeDocument codeDocument, DocumentIntermediateNode documentNode)
      {
        new InheritsVisitor().Visit(documentNode);
      }
    }

    private class InheritsVisitor : IntermediateNodeWalker
    {
      /// <inheritdoc />
      public override void VisitDirective(DirectiveIntermediateNode node)
      {
        if (node.Directive != InheritsDirective)
        {
          base.VisitDirective(node);
        }
      }
    }
  }
}