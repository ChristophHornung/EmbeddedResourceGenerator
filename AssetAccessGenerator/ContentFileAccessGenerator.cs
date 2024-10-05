namespace AssetAccessGenerator;

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

public static class ContentFileAccessGenerator
{
	public static void GenerateCode(SourceProductionContext context, GenerationContext generationContext)
	{
		var contentFiles = generationContext.With(ResourceKind.Content);

		if (contentFiles.IsEmpty)
		{
			return;
		}

		StringBuilder sourceBuilder = new();
		sourceBuilder.AppendLine($$"""
		                           #nullable enable
		                           namespace {{contentFiles.RootNamespace}};
		                           using System;
		                           using System.IO;

		                           /// <summary>
		                           /// Auto-generated class to access all content files in an assembly.
		                           /// </summary>
		                           public static partial class ContentFilesExtensions
		                           {
		                           """);

		//foreach ((string path, string identifierName, string resourceName) in contentFiles)
		//{
		//	sourceBuilder.AppendLine($$"""
		//	                           	/// <summary>
		//	                           	/// Gets the content file '{{resourceName}}' as a stream.
		//	                           	/// </summary>
		//	                           	/// <returns>The stream to access the content file.</returns>
		//	                           	public static Stream {{identifierName}}_Stream
		//	                           	{
		//	                           		get {
		//	                           			return File.OpenRead(@"{{path}}")!;
		//	                           		}
		//	                           	}

		//	                           	/// <summary>
		//	                           	/// Gets the content file '{{resourceName}}' as a stream-reader.
		//	                           	/// </summary>
		//	                           	/// <returns>The stream-reader to access the content file.</returns>
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
		                           	/// Gets the content file's stream.
		                           	/// </summary>
		                           	/// <param name="file">The content file to retrieve the stream for.</param>
		                           	/// <returns>The stream to access the content file.</returns>
		                           	public static Stream GetStream(this ContentFile file)
		                           	{
		                           		return File.OpenRead(GetContentFilePath(file))!;
		                           	}
		                           
		                           	/// <summary>
		                           	/// Gets the content file's stream-reader.
		                           	/// </summary>
		                           	/// <param name="file">The content file to retrieve the stream-reader for.</param>
		                           	/// <returns>The stream-reader to access the content file.</returns>
		                           	public static StreamReader GetReader(this ContentFile file)
		                           	{
		                           		return new StreamReader(File.OpenRead(GetContentFilePath(file))!, leaveOpen:false);
		                           	}
		                           	
		                           	/// <summary>
		                           	/// Reads the content file's text asynchronously.
		                           	/// </summary>
		                           	/// <param name="file">The content file to read all text.</param>
		                           	/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is System.Threading.CancellationToken.None.</param>
		                           	/// <returns>text.</returns>
		                           	public static async Task<string> ReadAllTextAsync(this ContentFile file, CancellationToken cancellationToken = default(CancellationToken))
		                           	{
		                           	    return await File.ReadAllTextAsync(GetContentFilePath(file), cancellationToken)!;
		                           	}
		                           	
		                           	/// <summary>
		                           	/// Reads the content file's text.
		                           	/// </summary>
		                           	/// <param name="file">The content file to read all text.</param>
		                           	/// <returns>text.</returns>
		                           	public static string ReadAllText(this ContentFile file)
		                           	{
		                           	    return File.ReadAllText(GetContentFilePath(file))!;
		                           	}
		                           	
		                           """);

		sourceBuilder.AppendLine("""
		                         	/// <summary>
		                         	/// Gets the content file's path.
		                         	/// </summary>
		                         	/// <param name="file">The content file to retrieve the name for.</param>
		                         	/// <returns>The path to access the content file.</returns>
		                         	public static string GetContentFilePath(this ContentFile file)
		                         	{
		                         		return file switch 
		                         		{
		                         """);

		foreach ((string path, string identifierName, string _, _) in contentFiles)
		{
			sourceBuilder.AppendLine($$"""
			                           			ContentFile.{{identifierName}} => @"{{path}}",
			                           """);
		}

		sourceBuilder.AppendLine("""			_ => throw new InvalidOperationException(),""");

		sourceBuilder.AppendLine("\t\t};");

		sourceBuilder.AppendLine("\t}");

		foreach (IGrouping<string, ResourceItem> pathGrouped in contentFiles.GroupBy(g =>
					 Path.GetDirectoryName(g.RelativePath)))
		{
			string pathAsClassName = Utils.PathAsClassname(pathGrouped.Key, "_");
			if (!string.IsNullOrEmpty(pathGrouped.Key))
			{
				sourceBuilder.AppendLine($$$"""
				                           
				                           	/// <summary>
				                           	/// Gets the content file's stream.
				                           	/// </summary>
				                           	/// <param name="file">The content file to retrieve the stream for.</param>
				                           	/// <returns>The stream to access the content file.</returns>
				                           	public static Stream GetStream(this ContentFile_{{{pathAsClassName}}} file)
				                           	{
				                           		return File.OpenRead(GetContentFilePath(file))!;
				                           	}
				                           
				                           	/// <summary>
				                           	/// Gets the content file's stream-reader.
				                           	/// </summary>
				                           	/// <param name="file">The content file to retrieve the stream-reader for.</param>
				                           	/// <returns>The stream-reader to access the content file.</returns>
				                           	public static StreamReader GetReader(this ContentFile_{{{pathAsClassName}}} file)
				                           	{
				                           		return new StreamReader(File.OpenRead(GetContentFilePath(file))!, leaveOpen:false);
				                           	}
				                           	
				                           	/// <summary>
				                            /// Reads the content file's text asynchronously.
				                            /// </summary>
				                            /// <param name="file">The content file to read all text.</param>
				                            /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is System.Threading.CancellationToken.None.</param>
				                            /// <returns>text.</returns>
				                           	public static async Task<string> ReadAllTextAsync(this ContentFile_{{{pathAsClassName}}} file, CancellationToken cancellationToken = default(CancellationToken))
				                           	{
				                           		return await File.ReadAllTextAsync(GetContentFilePath(file), cancellationToken)!;
				                           	}
				                           	
				                           	/// <summary>
				                           	/// Reads the content file's text.
				                           	/// </summary>
				                           	/// <param name="file">The content file to read all text.</param>
				                           	/// <returns>text.</returns>
				                           	public static string ReadAllText(this ContentFile_{{{pathAsClassName}}} file)
				                           	{
				                           		return File.ReadAllText(GetContentFilePath(file))!;
				                           	}
				                           	
				                           """);

				sourceBuilder.AppendLine($$"""
				                           
				                           	/// <summary>
				                           	/// Gets the content file's path.
				                           	/// </summary>
				                           	/// <param name="file">The content file to retrieve the name for.</param>
				                           	/// <returns>The name to access the content file.</returns>
				                           	public static string GetContentFilePath(this ContentFile_{{pathAsClassName}} file)
				                           	{
				                           		return file switch 
				                           		{
				                           """);

				foreach ((string relativePath, string identifierName, string resourceName, _) in pathGrouped)
				{
					string nonPathedIdentifierName = Utils.GetValidIdentifierName(Path.GetFileName(relativePath));

					sourceBuilder.AppendLine($$"""
					                           			ContentFile_{{pathAsClassName}}.{{nonPathedIdentifierName}} => @"{{relativePath}}",
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
		                         /// Auto-generated enumeration for all content files in the assembly.
		                         /// </summary>
		                         public enum ContentFile
		                         {
		                         """);

		foreach ((string _, string identifierName, string resourceName, _) in contentFiles)
		{
			sourceBuilder.AppendLine($$"""
			                           	/// <summary>
			                           	/// Represents the content file '{{resourceName}}'.
			                           	/// </summary>
			                           	{{identifierName}},
			                           """);
		}

		sourceBuilder.AppendLine("}");

		foreach (IGrouping<string, ResourceItem> pathGrouped in contentFiles.GroupBy(g =>
					 Path.GetDirectoryName(g.RelativePath)))
		{
			string pathAsClassName = Utils.PathAsClassname(pathGrouped.Key, "_");
			if (!string.IsNullOrEmpty(pathGrouped.Key))
			{
				sourceBuilder.AppendLine($$"""

				                           /// <summary>
				                           /// Auto-generated enumeration for all content files in '{{pathGrouped.Key}}'.
				                           /// </summary>
				                           public enum ContentFile_{{pathAsClassName}}
				                           {
				                           """);

				foreach (ResourceItem item in pathGrouped)
				{
					string nonPathedIdentifierName = Utils.GetValidIdentifierName(Path.GetFileName(item.RelativePath));

					sourceBuilder.AppendLine($$"""
					                           	/// <summary>
					                           	/// Represents the content file '{{Path.GetFileName(item.RelativePath)}}' in {{pathGrouped.Key}}.
					                           	/// </summary>
					                           	{{nonPathedIdentifierName}},
					                           """);
				}

				sourceBuilder.AppendLine("}");
			}
		}

		sourceBuilder.Append("#nullable restore");

		SourceText source = SourceText.From(sourceBuilder.ToString(), Encoding.UTF8);
		context.AddSource("ContentFiles.generated.cs", source);
	}
}