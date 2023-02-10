namespace EmbeddedResourceAccessGenerator;

using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

/// <summary>
/// The generator for the embedded resource access.
/// </summary>
[Generator]
public class EmbeddedResourceAccessGenerator : IIncrementalGenerator
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

	/// <inheritdoc />
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		//Debugger.Launch();

		// We need a value provider for any addition file.
		// As soon as there is direct access to embedded resources we can change this.
		// All embedded resources are added as additional files through our build props integrated into the nuget.
		IncrementalValueProvider<ImmutableArray<string>> additionaFilesProvider =
			context.AdditionalTextsProvider.Select((t, _) => t.Path).Collect();

		// The root namespace value provider. Can this ever be null? So far I have not seen it.
		IncrementalValueProvider<string?> rootNamespaceProvider = context.AnalyzerConfigOptionsProvider.Select((x, _) =>
			x.GlobalOptions.TryGetValue("build_property.RootNamespace", out string? rootNamespace)
				? rootNamespace
				: null);

		// The project directory value provider. Can this ever be null? So far I have not seen it.
		IncrementalValueProvider<string?> buildProjectDirProvider = context.AnalyzerConfigOptionsProvider.Select(
			(x, _) =>
				x.GlobalOptions.TryGetValue("build_property.projectdir", out string? rootNamespace)
					? rootNamespace
					: null);

		// We combine the providers to generate the parameters for our source generation.
		IncrementalValueProvider<(ImmutableArray<string> fileNames, string? rootNamespace, string? buildProjectDir)>
			combined = additionaFilesProvider
				.Combine(rootNamespaceProvider.Combine(buildProjectDirProvider)).Select((c, _) =>
					(c.Left, c.Right.Left, c.Right.Right));

		context.RegisterSourceOutput(combined, this.GenerateSourceIncremental);
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

	private void GenerateSourceIncremental(SourceProductionContext context,
		(ImmutableArray<string> fileNames, string? rootNamespace, string? buildProjectDir) arg2)
	{
		try
		{
			this.GenerateSource(context, arg2.fileNames, arg2.buildProjectDir!, arg2.rootNamespace);
		}
		catch (Exception e)
		{
			// We generate a diagnostic message on all internal failures.
			context.ReportDiagnostic(Diagnostic.Create(EmbeddedResourceAccessGenerator.generationWarning, Location.None,
				e.Message, e.StackTrace));
		}
	}

	private void GenerateSource(SourceProductionContext context, IList<string> paths, string mainDirectory,
		string? rootNamespace)
	{
		List<EmbeddedResourceItem> embeddedResources = new();
		foreach (string path in paths)
		{
			string resourcePath =
				EmbeddedResourceAccessGenerator.GetRelativePath(path, mainDirectory).Replace("%20"," ");

			string resourceName = this.GetResourceName(resourcePath);
			string identifierName = this.GetValidIdentifierName(resourcePath);
			embeddedResources.Add(new EmbeddedResourceItem(resourcePath, identifierName, resourceName));
		}

		StringBuilder sourceBuilder = new();
		sourceBuilder.AppendLine($$"""
				#nullable enable
				namespace {{rootNamespace}};
				using System;
				using System.Collections;
				using System.IO;
				using System.Reflection;

				/// <summary>
				/// Auto-generated class to access all embedded resources in an assembly.
				/// </summary>
				public static partial class EmbeddedResources
				{
				""");

		foreach ((string _, string identifierName, string resourceName) in embeddedResources)
		{
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

		sourceBuilder.AppendLine("""
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

		sourceBuilder.AppendLine("""
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

		foreach ((string _, string identifierName, string resourceName) in embeddedResources)
		{
			sourceBuilder.AppendLine($$"""
							EmbeddedResource.{{identifierName}} => "{{rootNamespace}}.{{resourceName}}",
				""");
		}

		sourceBuilder.AppendLine("""			_ => throw new InvalidOperationException(),""");

		sourceBuilder.AppendLine("\t\t};");

		sourceBuilder.AppendLine("\t}");

		foreach (IGrouping<string, EmbeddedResourceItem> pathGrouped in embeddedResources.GroupBy(g =>
			         Path.GetDirectoryName(g.RelativePath)))
		{
			string pathAsClassName = this.PathAsClassname(pathGrouped.Key);
			if (!string.IsNullOrEmpty(pathGrouped.Key))
			{
				sourceBuilder.AppendLine($$"""
				
					/// <summary>
					/// Gets the embedded resource's stream.
					/// </summary>
					/// <param name="resource">The embedded resource to retrieve the stream for.</param>
					/// <returns>The stream to access the embedded resource.</returns>
					public static Stream GetStream(this EmbeddedResource{{pathAsClassName}} resource)
					{
						Assembly assembly = typeof(EmbeddedResources).Assembly;
						return assembly.GetManifestResourceStream(GetResourceName(resource))!;
					}
				
					/// <summary>
					/// Gets the embedded resource's stream-reader.
					/// </summary>
					/// <param name="resource">The embedded resource to retrieve the stream-reader for.</param>
					/// <returns>The stream-reader to access the embedded resource.</returns>
					public static StreamReader GetReader(this EmbeddedResource{{pathAsClassName}} resource)
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
					public static string GetResourceName(this EmbeddedResource{{pathAsClassName}} resource)
					{
						return resource switch 
						{
				""");

				foreach ((string relativePath, string identifierName, string resourceName) in pathGrouped)
				{
					string nonPathedIdentifierName = this.GetValidIdentifierName(Path.GetFileName(relativePath));

					sourceBuilder.AppendLine($$"""
							EmbeddedResource{{pathAsClassName}}.{{nonPathedIdentifierName}} => "{{rootNamespace}}.{{resourceName}}",
				""");
				}

				sourceBuilder.AppendLine("""			_ => throw new InvalidOperationException(),""");

				sourceBuilder.AppendLine("\t\t};");

				sourceBuilder.AppendLine("\t}");
			}
		}

		sourceBuilder.AppendLine("}");

		sourceBuilder.AppendLine("""
				
				/// <summary>
				/// Auto-generated enumeration for all embedded resources in the assembly.
				/// </summary>
				public enum EmbeddedResource
				{
				""");

		foreach ((string _, string identifierName, string resourceName) in embeddedResources)
		{
			sourceBuilder.AppendLine($$"""
					/// <summary>
					/// Represents the embedded resource '{{resourceName}}'.
					/// </summary>
					{{identifierName}},
				""");
		}

		sourceBuilder.AppendLine("}");

		foreach (IGrouping<string, EmbeddedResourceItem> pathGrouped in embeddedResources.GroupBy(g =>
			         Path.GetDirectoryName(g.RelativePath)))
		{
			string pathAsClassName = this.PathAsClassname(pathGrouped.Key);
			if (!string.IsNullOrEmpty(pathGrouped.Key))
			{
				sourceBuilder.AppendLine($$"""
						
						/// <summary>
						/// Auto-generated enumeration for all embedded resources in '{{pathGrouped.Key}}'.
						/// </summary>
						public enum EmbeddedResource{{pathAsClassName}}
						{
						""");

				foreach (EmbeddedResourceItem item in pathGrouped)
				{
					string nonPathedIdentifierName = this.GetValidIdentifierName(Path.GetFileName(item.RelativePath));

					sourceBuilder.AppendLine($$"""
							/// <summary>
							/// Represents the embedded resource '{{Path.GetFileName(item.RelativePath)}}' in {{pathGrouped.Key}}.
							/// </summary>
							{{nonPathedIdentifierName}},
						""");
				}

				sourceBuilder.AppendLine("}");
			}
		}

		sourceBuilder.Append("#nullable restore");

		SourceText source = SourceText.From(sourceBuilder.ToString(), Encoding.UTF8);
		context.AddSource("EmbeddedResources.generated.cs", source);
	}

	private string GetResourceName(string resourcePath)
	{
		// return CreateManifestResourceName.MakeValidEverettIdentifier(resourcePath);
		// If we reference Microsoft.Build.Tasks.Core we get this error on our referencing projects:
		// Error	EMBRESGEN001	Exception 'Could not load file or assembly 'Microsoft.Build.Tasks.Core, Version=15.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
		// or one of its dependencies. The system cannot find the file specified.'
		// at EmbeddedResourceAccessGenerator.EmbeddedResourceAccessGenerator.GetResourceName(String resourcePath)

		// So instead we implement the code ourselves.
		StringBuilder manifestName = new();
		// See https://github.com/dotnet/msbuild/blob/7cfb36cb90d1c9cc34bc4e0910d0c9ef42ee47b6/src/Tasks/CreateCSharpManifestResourceName.cs#L152
		string? directoryName = Path.GetDirectoryName(resourcePath);
		manifestName.Append(EverettIdentifierHelper.MakeValidEverettIdentifier(directoryName));
		if (!string.IsNullOrEmpty(directoryName))
		{
			manifestName.Append('.');
		}

		manifestName.Append(Path.GetFileName(resourcePath));

		return manifestName.ToString();
	}

	private string PathAsClassname(string path)
	{
		return this.GetValidIdentifierName(path.Replace("\\", string.Empty).Replace("/", string.Empty));
	}

	private string GetValidEverettIdentifierName(string resourceName)
	{
		StringBuilder sb = new(resourceName);

		bool first = true;
		for (int index = 0; index < resourceName.Length; index++)
		{
			char c = resourceName[index];
			bool replace = first
				? !EverettIdentifierHelper.IsValidEverettIdFirstChar(c)
				: !EverettIdentifierHelper.IsValidEverettIdChar(c);

			if (replace)
			{
				sb[index] = '_';
			}

			first = false;
		}

		return sb.ToString();
	}

	private string GetValidIdentifierName(string resourceName)
	{
		StringBuilder sb = new(resourceName);
		sb.Replace('.', '_');

		bool first = true;
		for (int index = 0; index < resourceName.Length; index++)
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


	private void Log(GeneratorExecutionContext context, string log)
	{
#if DEBUG
		context.ReportDiagnostic(Diagnostic.Create(EmbeddedResourceAccessGenerator.logInfo, Location.None, log));
#endif
	}
}