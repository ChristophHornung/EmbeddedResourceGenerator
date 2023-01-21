namespace EmbeddedResourceAccessGenerator;

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

/// <summary>
/// The generator for the embedded resource access.
/// </summary>
[Generator]
public class EmbeddedResourceAccessGenerator : ISourceGenerator
{
	private static readonly DiagnosticDescriptor generationWarning = new DiagnosticDescriptor(
		id: "EMBRESGEN001",
		title: "Exception on generation",
		messageFormat: "Exception '{0}' {1}",
		category: "MessageExtensionGenerator",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);

#if DEBUG
	private static readonly DiagnosticDescriptor logInfo = new DiagnosticDescriptor(
		id: "EMBRESGENLOG",
		title: "Log",
		messageFormat: "{0}",
		category: "MessageExtensionGenerator",
		DiagnosticSeverity.Warning,
		isEnabledByDefault: true);
#endif

	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		SyntaxTree mainSyntaxTree = context.Compilation.SyntaxTrees
			.First(x => x.HasCompilationUnitRoot);

		string mainDirectory = Path.GetDirectoryName(mainSyntaxTree.FilePath)!;

		context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.rootnamespace", out var rootNamespace);

		try
		{
			StringBuilder sourceBuilder = new();
			sourceBuilder.AppendLine($$"""
				#nullable enable
				namespace {{rootNamespace}};
				using System.Collections;
				using System.IO;
				using System.Reflection;

				public static partial class EmbeddedResources
				{
				""");


			foreach (AdditionalText contextAdditionalFile in context.AdditionalFiles)
			{
				string resourceName =
					EmbeddedResourceAccessGenerator.GetRelativePath(contextAdditionalFile.Path, mainDirectory);
				resourceName = resourceName.Replace('\\', '.').Replace('/', '.');
				sourceBuilder.AppendLine($$"""
					public static Stream {{resourceName.Replace('.', '_')}}_Stream
					{
						get {
							Assembly assembly = typeof(EmbeddedResources).Assembly;
							string resource = "{{rootNamespace}}.{{resourceName}}";
							return assembly.GetManifestResourceStream(resource)!;
						}
					}

					public static StreamReader {{resourceName.Replace('.', '_')}}_Reader
					{
						get 
						{
							Assembly assembly = typeof(EmbeddedResources).Assembly;
							string resource = "{{rootNamespace}}.{{resourceName}}";
							return new StreamReader(assembly.GetManifestResourceStream(resource)!);
						}
					}

				""");
			}

			sourceBuilder.AppendLine($$"""
					public static Stream GetStream(this EmbeddedResource resource)
					{
						Assembly assembly = typeof(EmbeddedResources).Assembly;
						return assembly.GetManifestResourceStream(GetResourceName(resource))!;
					}
				
					public static StreamReader GetReader(this EmbeddedResource resource)
					{
						Assembly assembly = typeof(EmbeddedResources).Assembly;
						return new StreamReader(assembly.GetManifestResourceStream(GetResourceName(resource))!);
					}
				
					public static string GetResourceName(this EmbeddedResource resource)
					{
						return "{{rootNamespace}}." + Enum.GetName(typeof(EmbeddedResource), resource)!.Replace('_','.');
					}
				""");

			sourceBuilder.AppendLine("}");

			sourceBuilder.AppendLine("""
				
				public enum EmbeddedResource
				{
				""");

			foreach (AdditionalText contextAdditionalFile in context.AdditionalFiles)
			{
				string resourceName =
					EmbeddedResourceAccessGenerator.GetRelativePath(contextAdditionalFile.Path, mainDirectory);
				resourceName = resourceName.Replace('\\', '.').Replace('/', '.');
				sourceBuilder.AppendLine($$"""
					{{resourceName.Replace('.', '_')}},
				""");
			}

			sourceBuilder.AppendLine("}");
			sourceBuilder.AppendLine("#nullable restore");

			SourceText source = SourceText.From(sourceBuilder.ToString(), Encoding.UTF8);
			context.AddSource("EmbeddedResourcesgenerated.cs", source);
		}
		catch (Exception e)
		{
			context.ReportDiagnostic(Diagnostic.Create(EmbeddedResourceAccessGenerator.generationWarning, Location.None,
				e.Message, e.StackTrace));
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
	{
		// Debugger.Launch();
	}

	private static string GetRelativePath(string fullPath, string basePath)
	{
		if (!basePath.EndsWith("\\"))
		{
			basePath += "\\";
		}

		Uri baseUri = new(basePath);
		Uri fullUri = new(fullPath);

		Uri relativeUri = baseUri.MakeRelativeUri(fullUri);

		return relativeUri.ToString().Replace("/", "\\");
	}

	private void Log(GeneratorExecutionContext context, string log)
	{
#if DEBUG
		context.ReportDiagnostic(Diagnostic.Create(EmbeddedResourceAccessGenerator.logInfo, Location.None, log));
#endif
	}
}