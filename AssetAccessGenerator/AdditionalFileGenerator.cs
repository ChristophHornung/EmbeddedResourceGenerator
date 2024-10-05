namespace AssetAccessGenerator;

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

public static class AdditionalFileGenerator
{
	public static void GenerateCode(SourceProductionContext context, GenerationContext generationContext)
	{
		var additionalFilesContext = generationContext.With(ResourceKind.AdditionalFile);

		if (additionalFilesContext.IsEmpty)
		{
			return;
		}

		StringBuilder sourceBuilder = new();
		sourceBuilder.AppendLine($$"""
		                           #nullable enable
		                           namespace {{additionalFilesContext.RootNamespace}};
		                           using System;
		                           using System.IO;

		                           /// <summary>
		                           /// Auto-generated class to access all additional files in an assembly.
		                           /// </summary>
		                           public static partial class AdditionalFilesExtensions
		                           {
		                           """);

		//foreach ((string path, string identifierName, string resourceName) in additionalFiles)
		//{
		//	sourceBuilder.AppendLine($$"""
		//	                           	/// <summary>
		//	                           	/// Gets the additional file '{{resourceName}}' as a stream.
		//	                           	/// </summary>
		//	                           	/// <returns>The stream to access the additional file.</returns>
		//	                           	public static Stream {{identifierName}}_Stream
		//	                           	{
		//	                           		get {
		//	                           			return File.OpenRead(@"{{path}}")!;
		//	                           		}
		//	                           	}

		//	                           	/// <summary>
		//	                           	/// Gets the additional file '{{resourceName}}' as a stream-reader.
		//	                           	/// </summary>
		//	                           	/// <returns>The stream-reader to access the additional file.</returns>
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
		                           	/// Gets the additional file's stream.
		                           	/// </summary>
		                           	/// <param name="file">The additional file to retrieve the stream for.</param>
		                           	/// <returns>The stream to access the additional file.</returns>
		                           	public static Stream GetStream(this AdditionalFile file)
		                           	{
		                           		return File.OpenRead(GetAdditionalFilePath(file))!;
		                           	}
		                           
		                           	/// <summary>
		                           	/// Gets the additional file's stream-reader.
		                           	/// </summary>
		                           	/// <param name="file">The additional file to retrieve the stream-reader for.</param>
		                           	/// <returns>The stream-reader to access the additional file.</returns>
		                           	public static StreamReader GetReader(this AdditionalFile file)
		                           	{
		                           		return new StreamReader(File.OpenRead(GetAdditionalFilePath(file))!, leaveOpen:false);
		                           	}
		                           	
		                           	/// <summary>
		                           	/// Reads the additional file's text asynchronously.
		                           	/// </summary>
		                           	/// <param name="file">The additional file to read all text.</param>
		                           	/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is System.Threading.CancellationToken.None.</param>
		                           	/// <returns>text.</returns>
		                           	public static async Task<string> ReadAllTextAsync(this AdditionalFile file, CancellationToken cancellationToken = default(CancellationToken))
		                           	{
		                           	    return await File.ReadAllTextAsync(GetAdditionalFilePath(file), cancellationToken)!;
		                           	}
		                           	
		                           	/// <summary>
		                           	/// Reads the additional file's text.
		                           	/// </summary>
		                           	/// <param name="resource">The additional file to read all text.</param>
		                           	/// <returns>text.</returns>
		                           	public static string ReadAllText(this AdditionalFile file)
		                           	{
		                           	    return File.ReadAllText(GetAdditionalFilePath(file))!;
		                           	}
		                           	
		                           """);

		sourceBuilder.AppendLine("""
		                         	/// <summary>
		                         	/// Gets the additional file's path.
		                         	/// </summary>
		                         	/// <param name="file">The additional file to retrieve the name for.</param>
		                         	/// <returns>The path to access the additional file.</returns>
		                         	public static string GetAdditionalFilePath(this AdditionalFile file)
		                         	{
		                         		return file switch 
		                         		{
		                         """);

		foreach ((string path, string identifierName, string _, _) in additionalFilesContext)
		{
			sourceBuilder.AppendLine($$"""
			                           			AdditionalFile.{{identifierName}} => @"{{path}}",
			                           """);
		}

		sourceBuilder.AppendLine("""			_ => throw new InvalidOperationException(),""");

		sourceBuilder.AppendLine("\t\t};");

		sourceBuilder.AppendLine("\t}");

		foreach (IGrouping<string, ResourceItem> pathGrouped in additionalFilesContext.GroupBy(g =>
					 Path.GetDirectoryName(g.RelativePath)))
		{
			string pathAsClassName = Utils.PathAsClassname(pathGrouped.Key, "_");
			if (!string.IsNullOrEmpty(pathGrouped.Key))
			{
				sourceBuilder.AppendLine($$$"""
				                           
				                           	/// <summary>
				                           	/// Gets the additional file's stream.
				                           	/// </summary>
				                           	/// <param name="file">The additional file to retrieve the stream for.</param>
				                           	/// <returns>The stream to access the additional file.</returns>
				                           	public static Stream GetStream(this AdditionalFile_{{{pathAsClassName}}} file)
				                           	{
				                           		return File.OpenRead(GetAdditionalFilePath(file))!;
				                           	}
				                           
				                           	/// <summary>
				                           	/// Gets the additional file's stream-reader.
				                           	/// </summary>
				                           	/// <param name="file">The additional file to retrieve the stream-reader for.</param>
				                           	/// <returns>The stream-reader to access the additional file.</returns>
				                           	public static StreamReader GetReader(this AdditionalFile_{{{pathAsClassName}}} file)
				                           	{
				                           		return new StreamReader(File.OpenRead(GetAdditionalFilePath(file))!, leaveOpen:false);
				                           	}
				                           	
				                           	/// <summary>
				                            /// Reads the additional file's text asynchronously.
				                            /// </summary>
				                            /// <param name="file">The additional file to read all text.</param>
				                            /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is System.Threading.CancellationToken.None.</param>
				                            /// <returns>text.</returns>
				                           	public static async Task<string> ReadAllTextAsync(this AdditionalFile_{{{pathAsClassName}}} file, CancellationToken cancellationToken = default(CancellationToken))
				                           	{
				                           		return await File.ReadAllTextAsync(GetAdditionalFilePath(file), cancellationToken)!;
				                           	}
				                           	
				                           	/// <summary>
				                           	/// Reads the additional file's text.
				                           	/// </summary>
				                           	/// <param name="file">The additional file to read all text.</param>
				                           	/// <returns>text.</returns>
				                           	public static string ReadAllText(this AdditionalFile_{{{pathAsClassName}}} file)
				                           	{
				                           		return File.ReadAllText(GetAdditionalFilePath(file))!;
				                           	}
				                           	
				                           """);

				sourceBuilder.AppendLine($$"""
				                           
				                           	/// <summary>
				                           	/// Gets the additional file's name in the format required by <c>GetManifestResourceStream</c>.
				                           	/// </summary>
				                           	/// <param name="file">The additional file to retrieve the name for.</param>
				                           	/// <returns>The name to access the additional file.</returns>
				                           	public static string GetAdditionalFilePath(this AdditionalFile_{{pathAsClassName}} file)
				                           	{
				                           		return file switch 
				                           		{
				                           """);

				foreach ((string relativePath, string identifierName, string resourceName, _) in pathGrouped)
				{
					string nonPathedIdentifierName = Utils.GetValidIdentifierName(Path.GetFileName(relativePath));

					sourceBuilder.AppendLine($$"""
					                           			AdditionalFile_{{pathAsClassName}}.{{nonPathedIdentifierName}} => @"{{relativePath}}",
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
		                         /// Auto-generated enumeration for all additional files in the assembly.
		                         /// </summary>
		                         public enum AdditionalFile
		                         {
		                         """);

		foreach ((string _, string identifierName, string resourceName, _) in additionalFilesContext)
		{
			sourceBuilder.AppendLine($$"""
			                           	/// <summary>
			                           	/// Represents the additional file '{{resourceName}}'.
			                           	/// </summary>
			                           	{{identifierName}},
			                           """);
		}

		sourceBuilder.AppendLine("}");

		foreach (IGrouping<string, ResourceItem> pathGrouped in additionalFilesContext.GroupBy(g =>
					 Path.GetDirectoryName(g.RelativePath)))
		{
			string pathAsClassName = Utils.PathAsClassname(pathGrouped.Key, "_");
			if (!string.IsNullOrEmpty(pathGrouped.Key))
			{
				sourceBuilder.AppendLine($$"""

				                           /// <summary>
				                           /// Auto-generated enumeration for all additional files in '{{pathGrouped.Key}}'.
				                           /// </summary>
				                           public enum AdditionalFile_{{pathAsClassName}}
				                           {
				                           """);

				foreach (ResourceItem item in pathGrouped)
				{
					string nonPathedIdentifierName = Utils.GetValidIdentifierName(Path.GetFileName(item.RelativePath));

					sourceBuilder.AppendLine($$"""
					                           	/// <summary>
					                           	/// Represents the additional file '{{Path.GetFileName(item.RelativePath)}}' in {{pathGrouped.Key}}.
					                           	/// </summary>
					                           	{{nonPathedIdentifierName}},
					                           """);
				}

				sourceBuilder.AppendLine("}");
			}
		}

		sourceBuilder.Append("#nullable restore");

		SourceText source = SourceText.From(sourceBuilder.ToString(), Encoding.UTF8);
		context.AddSource("AdditionalFiles.generated.cs", source);
	}
}