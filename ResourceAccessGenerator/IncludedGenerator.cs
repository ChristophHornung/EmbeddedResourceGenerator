namespace ResourceAccessGenerator;

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

public static class IncludedGenerator
{
	public static void GenerateCode(SourceProductionContext context, ResourceGenerationContext resourcesContext)
	{
		var includedResources = resourcesContext.With(ResourceKind.Included);

		if (includedResources.IsEmpty)
		{
			return;
		}

		StringBuilder sourceBuilder = new();
		sourceBuilder.AppendLine($$"""
		                           #nullable enable
		                           namespace {{includedResources.RootNamespace}};
		                           using System;
		                           using System.IO;

		                           /// <summary>
		                           /// Auto-generated class to access all included resources in an assembly.
		                           /// </summary>
		                           public static partial class IncludedResourcesExtensions
		                           {
		                           """);

		//foreach ((string path, string identifierName, string resourceName) in includedResources)
		//{
		//	sourceBuilder.AppendLine($$"""
		//	                           	/// <summary>
		//	                           	/// Gets the included resource '{{resourceName}}' as a stream.
		//	                           	/// </summary>
		//	                           	/// <returns>The stream to access the included resource.</returns>
		//	                           	public static Stream {{identifierName}}_Stream
		//	                           	{
		//	                           		get {
		//	                           			return File.OpenRead(@"{{path}}")!;
		//	                           		}
		//	                           	}

		//	                           	/// <summary>
		//	                           	/// Gets the included resource '{{resourceName}}' as a stream-reader.
		//	                           	/// </summary>
		//	                           	/// <returns>The stream-reader to access the included resource.</returns>
		//	                           	public static StreamReader {{identifierName}}_Reader
		//	                           	{
		//	                           		get 
		//	                           		{
		//	                           			return new StreamReader(File.OpenRead(@"{{path}}")!, leaveOpen:false);
		//	                           		}
		//	                           	}

		//	                           """);
		//}

		sourceBuilder.AppendLine($$$$"""
		                           	/// <summary>
		                           	/// Gets the included resource's stream.
		                           	/// </summary>
		                           	/// <param name="resource">The included resource to retrieve the stream for.</param>
		                           	/// <returns>The stream to access the included resource.</returns>
		                           	public static Stream GetStream(this IncludedResource resource)
		                           	{
		                           		return File.OpenRead(GetResourcePath(resource))!;
		                           	}
		                           
		                           	/// <summary>
		                           	/// Gets the included resource's stream-reader.
		                           	/// </summary>
		                           	/// <param name="resource">The included resource to retrieve the stream-reader for.</param>
		                           	/// <returns>The stream-reader to access the included resource.</returns>
		                           	public static StreamReader GetReader(this IncludedResource resource)
		                           	{
		                           		return new StreamReader(File.OpenRead(GetResourcePath(resource))!, leaveOpen:false);
		                           	}
		                           	
		                           	/// <summary>
		                           	/// Reads the included resource's text asynchronously.
		                           	/// </summary>
		                           	/// <param name="resource">The included resource to retrieve the stream-reader for.</param>
		                           	/// <returns>text.</returns>
		                           	public static async Task<string> ReadAllTextAsync(this IncludedResource resource)
		                           	{
		                           	    return await File.ReadAllTextAsync(GetResourcePath(resource))!;
		                           	}
		                           	
		                           	/// <summary>
		                           	/// Reads the included resource's text.
		                           	/// </summary>
		                           	/// <param name="resource">The included resource to retrieve the stream-reader for.</param>
		                           	/// <returns>text.</returns>
		                           	public static string ReadAllText(this IncludedResource resource)
		                           	{
		                           	    return File.ReadAllText(GetResourcePath(resource))!;
		                           	}
		                           	
		                           """);

		sourceBuilder.AppendLine("""
		                         	/// <summary>
		                         	/// Gets the included resource's path.
		                         	/// </summary>
		                         	/// <param name="resource">The included resource to retrieve the name for.</param>
		                         	/// <returns>The path to access the included resource.</returns>
		                         	public static string GetResourcePath(this IncludedResource resource)
		                         	{
		                         		return resource switch 
		                         		{
		                         """);

		foreach ((string path, string identifierName, string _, _) in includedResources)
		{
			sourceBuilder.AppendLine($$"""
			                           			IncludedResource.{{identifierName}} => @"{{path}}",
			                           """);
		}

		sourceBuilder.AppendLine("""			_ => throw new InvalidOperationException(),""");

		sourceBuilder.AppendLine("\t\t};");

		sourceBuilder.AppendLine("\t}");

		foreach (IGrouping<string, ResourceItem> pathGrouped in includedResources.GroupBy(g =>
					 Path.GetDirectoryName(g.RelativePath)))
		{
			string pathAsClassName = Utils.PathAsClassname(pathGrouped.Key, "_");
			if (!string.IsNullOrEmpty(pathGrouped.Key))
			{
				sourceBuilder.AppendLine($$$"""
				                           
				                           	/// <summary>
				                           	/// Gets the included resource's stream.
				                           	/// </summary>
				                           	/// <param name="resource">The included resource to retrieve the stream for.</param>
				                           	/// <returns>The stream to access the included resource.</returns>
				                           	public static Stream GetStream(this IncludedResource_{{{pathAsClassName}}} resource)
				                           	{
				                           		return File.OpenRead(GetResourcePath(resource))!;
				                           	}
				                           
				                           	/// <summary>
				                           	/// Gets the included resource's stream-reader.
				                           	/// </summary>
				                           	/// <param name="resource">The included resource to retrieve the stream-reader for.</param>
				                           	/// <returns>The stream-reader to access the included resource.</returns>
				                           	public static StreamReader GetReader(this IncludedResource_{{{pathAsClassName}}} resource)
				                           	{
				                           		return new StreamReader(File.OpenRead(GetResourcePath(resource))!, leaveOpen:false);
				                           	}
				                           	
				                           	/// <summary>
				                           	/// Reads the included resource's text asynchronously.
				                           	/// </summary>
				                           	/// <param name="resource">The included resource to retrieve the stream-reader for.</param>
				                           	/// <returns>text.</returns>
				                           	public static async Task<string> ReadAllTextAsync(this IncludedResource_{{{pathAsClassName}}} resource)
				                           	{
				                           		return await File.ReadAllTextAsync(GetResourcePath(resource))!;
				                           	}
				                           	
				                           	/// <summary>
				                           	/// Reads the included resource's text.
				                           	/// </summary>
				                           	/// <param name="resource">The included resource to retrieve the stream-reader for.</param>
				                           	/// <returns>text.</returns>
				                           	public static string ReadAllText(this IncludedResource_{{{pathAsClassName}}} resource)
				                           	{
				                           		return File.ReadAllText(GetResourcePath(resource))!;
				                           	}
				                           	
				                           """);

				sourceBuilder.AppendLine($$"""
				                           
				                           	/// <summary>
				                           	/// Gets the included resource's name in the format required by <c>GetManifestResourceStream</c>.
				                           	/// </summary>
				                           	/// <param name="resource">The included resource to retrieve the name for.</param>
				                           	/// <returns>The name to access the included resource.</returns>
				                           	public static string GetResourcePath(this IncludedResource_{{pathAsClassName}} resource)
				                           	{
				                           		return resource switch 
				                           		{
				                           """);

				foreach ((string relativePath, string identifierName, string resourceName, _) in pathGrouped)
				{
					string nonPathedIdentifierName = Utils.GetValidIdentifierName(Path.GetFileName(relativePath));

					sourceBuilder.AppendLine($$"""
					                           			IncludedResource_{{pathAsClassName}}.{{nonPathedIdentifierName}} => @"{{relativePath}}",
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
		                         /// Auto-generated enumeration for all included resources in the assembly.
		                         /// </summary>
		                         public enum IncludedResource
		                         {
		                         """);

		foreach ((string _, string identifierName, string resourceName, _) in includedResources)
		{
			sourceBuilder.AppendLine($$"""
			                           	/// <summary>
			                           	/// Represents the included resource '{{resourceName}}'.
			                           	/// </summary>
			                           	{{identifierName}},
			                           """);
		}

		sourceBuilder.AppendLine("}");

		foreach (IGrouping<string, ResourceItem> pathGrouped in includedResources.GroupBy(g =>
					 Path.GetDirectoryName(g.RelativePath)))
		{
			string pathAsClassName = Utils.PathAsClassname(pathGrouped.Key, "_");
			if (!string.IsNullOrEmpty(pathGrouped.Key))
			{
				sourceBuilder.AppendLine($$"""

				                           /// <summary>
				                           /// Auto-generated enumeration for all included resources in '{{pathGrouped.Key}}'.
				                           /// </summary>
				                           public enum IncludedResource_{{pathAsClassName}}
				                           {
				                           """);

				foreach (ResourceItem item in pathGrouped)
				{
					string nonPathedIdentifierName = Utils.GetValidIdentifierName(Path.GetFileName(item.RelativePath));

					sourceBuilder.AppendLine($$"""
					                           	/// <summary>
					                           	/// Represents the included resource '{{Path.GetFileName(item.RelativePath)}}' in {{pathGrouped.Key}}.
					                           	/// </summary>
					                           	{{nonPathedIdentifierName}},
					                           """);
				}

				sourceBuilder.AppendLine("}");
			}
		}

		sourceBuilder.Append("#nullable restore");

		SourceText source = SourceText.From(sourceBuilder.ToString(), Encoding.UTF8);
		context.AddSource("IncludedResources.generated.cs", source);
	}
}