namespace EmbeddedResourceAccessGenerator;

using System.Globalization;
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

				/// <summary>
				/// Auto-generated class to access all embedded resources in an assembly.
				/// </summary>
				public static partial class EmbeddedResources
				{
				""");


			foreach (AdditionalText contextAdditionalFile in context.AdditionalFiles)
			{
				string resourceName =
					EmbeddedResourceAccessGenerator.GetRelativePath(contextAdditionalFile.Path, mainDirectory);
				resourceName = this.GetResourceName(resourceName);
				string identifierName = this.GetValidIdentifierName(resourceName);

				sourceBuilder.AppendLine($$"""
					/// <summary>
					/// Gets the embedded resource '{{resourceName}}' as a stream.
					/// </summary>
					/// <returns>The stream to access the embedded resource.</returns>
					public static Stream {{identifierName}}_Stream
					{
						get {
							Assembly assembly = typeof(EmbeddedResources).Assembly;
							string resource = "{{rootNamespace}}.{{resourceName}}";
							return assembly.GetManifestResourceStream(resource)!;
						}
					}

					/// <summary>
					/// Gets the embedded resource '{{resourceName}}' as a stream-reader.
					/// </summary>
					/// <returns>The stream-reader to access the embedded resource.</returns>
					public static StreamReader {{identifierName}}_Reader
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
					/// <summary>
					/// Gets the embedded resource's stream.
					/// </summary>
					/// <param name="resource">The embedded resource to retrieve the stream for.</param>
					/// <returns>The stream to access the embedded resource.</returns>
					public static Stream GetStream(this EmbeddedResource resource)
					{
						Assembly assembly = typeof(EmbeddedResources).Assembly;
						return assembly.GetManifestResourceStream(GetResourceName(resource))!;
					}
				
					/// <summary>
					/// Gets the embedded resource's stream-reader.
					/// </summary>
					/// <param name="resource">The embedded resource to retrieve the stream-reader for.</param>
					/// <returns>The stream-reader to access the embedded resource.</returns>
					public static StreamReader GetReader(this EmbeddedResource resource)
					{
						Assembly assembly = typeof(EmbeddedResources).Assembly;
						return new StreamReader(assembly.GetManifestResourceStream(GetResourceName(resource))!);
					}

				""");

			sourceBuilder.AppendLine($$"""
					/// <summary>
					/// Gets the embedded resource's name in the format required by <c>GetManifestResourceStream</c>.
					/// </summary>
					/// <param name="resource">The embedded resource to retrieve the name for.</param>
					/// <returns>The name to access the embedded resource.</returns>
					public static string GetResourceName(this EmbeddedResource resource)
					{
						return resource switch 
						{
				""");

			foreach (AdditionalText contextAdditionalFile in context.AdditionalFiles)
			{
				string resourceName =
					EmbeddedResourceAccessGenerator.GetRelativePath(contextAdditionalFile.Path, mainDirectory);
				resourceName = this.GetResourceName(resourceName);
				string identifierName = this.GetValidIdentifierName(resourceName);

				sourceBuilder.AppendLine($$"""
							EmbeddedResource.{{identifierName}} => "{{rootNamespace}}.{{resourceName}}",
				""");
			}

			sourceBuilder.AppendLine($$"""
							_ => throw new InvalidOperationException(),
				""");

			sourceBuilder.AppendLine("\t\t};");

			sourceBuilder.AppendLine("\t}");

			sourceBuilder.AppendLine("}");

			sourceBuilder.AppendLine("""
				/// <summary>
				/// Auto-generated enumeration for all embedded resources in the assembly.
				/// </summary>
				public enum EmbeddedResource
				{
				""");

			foreach (AdditionalText contextAdditionalFile in context.AdditionalFiles)
			{
				string resourceName =
					EmbeddedResourceAccessGenerator.GetRelativePath(contextAdditionalFile.Path, mainDirectory);
				resourceName = this.GetResourceName(resourceName);
				string identifierName = this.GetValidIdentifierName(resourceName);
				sourceBuilder.AppendLine($$"""
					/// <summary>
					/// Represents the embedded resource '{{resourceName}}'.
					/// </summary>
					{{identifierName}},
				""");
			}

			sourceBuilder.AppendLine("}");
			sourceBuilder.AppendLine("#nullable restore");

			SourceText source = SourceText.From(sourceBuilder.ToString(), Encoding.UTF8);
			context.AddSource("EmbeddedResources.generated.cs", source);
		}
		catch (Exception e)
		{
			context.ReportDiagnostic(Diagnostic.Create(EmbeddedResourceAccessGenerator.generationWarning, Location.None,
				e.Message, e.StackTrace));
		}
	}

	private string GetResourceName(string resourceName)
	{
		return resourceName.Replace('\\', '.').Replace('/', '.');
	}

	private string GetValidIdentifierName(string resourceName)
	{
		StringBuilder sb = new(resourceName);
		sb.Replace('.', '_');

		bool first = true;
		for (var index = 0; index < resourceName.Length; index++)
		{
			char c = resourceName[index];
			bool replace;
			switch (char.GetUnicodeCategory(c))
			{
				case UnicodeCategory.LowercaseLetter:
				case UnicodeCategory.UppercaseLetter:
				case UnicodeCategory.TitlecaseLetter:
				case UnicodeCategory.ModifierLetter:
				case UnicodeCategory.OtherLetter:
					replace = false;
					break;
				case UnicodeCategory.ConnectorPunctuation:
				case UnicodeCategory.DecimalDigitNumber:
				case UnicodeCategory.Format:
				case UnicodeCategory.LetterNumber:
				case UnicodeCategory.NonSpacingMark:
				case UnicodeCategory.SpacingCombiningMark:
					// Only valid in non-leading position.
					replace = first;
					break;
				default:
					replace = true;
					break;
			}

			if (replace)
			{
				sb[index] = '_';
			}

			first = false;
		}
		
		return sb.ToString();
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
	{
		// Debugger.Launch();
	}

	private static string GetRelativePath(string fullPath, string basePath)
	{
		if (!basePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
		{
			basePath += Path.DirectorySeparatorChar;
		}

		Uri baseUri = new(basePath);
		Uri fullUri = new(fullPath);

		Uri relativeUri = baseUri.MakeRelativeUri(fullUri);

		return relativeUri.ToString().Replace("/", Path.DirectorySeparatorChar.ToString());
	}

	private void Log(GeneratorExecutionContext context, string log)
	{
#if DEBUG
		context.ReportDiagnostic(Diagnostic.Create(EmbeddedResourceAccessGenerator.logInfo, Location.None, log));
#endif
	}
}