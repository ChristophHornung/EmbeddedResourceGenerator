namespace ResourceAccessGenerator;

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

public static class EmbeddedGenerator
{
	public static void GenerateCode(SourceProductionContext context, ResourceGenerationContext resourcesContext)
	{
		var embeddedResources = resourcesContext.With(ResourceKind.Embedded);

		if (embeddedResources.IsEmpty)
		{
			return;
		}

		StringBuilder sourceBuilder = new();
		sourceBuilder.AppendLine($$"""
		                           #nullable enable
		                           namespace {{embeddedResources.RootNamespace}};
		                           using System;
		                           using System.IO;
		                           using System.Reflection;

		                           /// <summary>
		                           /// Auto-generated class to access all embedded resources in an assembly.
		                           /// </summary>
		                           public static partial class EmbeddedResources
		                           {
		                           """);

		foreach ((string _, string identifierName, string resourceName, _) in embeddedResources)
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
			                           			string resource = "{{embeddedResources.RootNamespace}}.{{resourceName}}";
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
			                           			string resource = "{{embeddedResources.RootNamespace}}.{{resourceName}}";
			                           			return new StreamReader(assembly.GetManifestResourceStream(resource)!, leaveOpen:false);
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
		                         		return new StreamReader(assembly.GetManifestResourceStream(GetResourceName(resource))!, leaveOpen:false);
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

		foreach ((string _, string identifierName, string resourceName, _) in embeddedResources)
		{
			sourceBuilder.AppendLine($$"""
			                           			EmbeddedResource.{{identifierName}} => "{{embeddedResources.RootNamespace}}.{{resourceName}}",
			                           """);
		}

		sourceBuilder.AppendLine("""			_ => throw new InvalidOperationException(),""");

		sourceBuilder.AppendLine("\t\t};");

		sourceBuilder.AppendLine("\t}");

		foreach (IGrouping<string, ResourceItem> pathGrouped in embeddedResources.GroupBy(g =>
					 Path.GetDirectoryName(g.RelativePath)))
		{
			string pathAsClassName = Utils.PathAsClassname(pathGrouped.Key);
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
				                           		return new StreamReader(assembly.GetManifestResourceStream(GetResourceName(resource))!, leaveOpen:false);
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

				foreach ((string relativePath, string identifierName, string resourceName, _) in pathGrouped)
				{
					string nonPathedIdentifierName = Utils.GetValidIdentifierName(Path.GetFileName(relativePath));

					sourceBuilder.AppendLine($$"""
					                           			EmbeddedResource{{pathAsClassName}}.{{nonPathedIdentifierName}} => "{{embeddedResources.RootNamespace}}.{{resourceName}}",
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

		foreach ((string _, string identifierName, string resourceName, _) in embeddedResources)
		{
			sourceBuilder.AppendLine($$"""
			                           	/// <summary>
			                           	/// Represents the embedded resource '{{resourceName}}'.
			                           	/// </summary>
			                           	{{identifierName}},
			                           """);
		}

		sourceBuilder.AppendLine("}");

		foreach (IGrouping<string, ResourceItem> pathGrouped in embeddedResources.GroupBy(g =>
					 Path.GetDirectoryName(g.RelativePath)))
		{
			string pathAsClassName = Utils.PathAsClassname(pathGrouped.Key);
			if (!string.IsNullOrEmpty(pathGrouped.Key))
			{
				sourceBuilder.AppendLine($$"""

				                           /// <summary>
				                           /// Auto-generated enumeration for all embedded resources in '{{pathGrouped.Key}}'.
				                           /// </summary>
				                           public enum EmbeddedResource{{pathAsClassName}}
				                           {
				                           """);

				foreach (ResourceItem item in pathGrouped)
				{
					string nonPathedIdentifierName = Utils.GetValidIdentifierName(Path.GetFileName(item.RelativePath));

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
}