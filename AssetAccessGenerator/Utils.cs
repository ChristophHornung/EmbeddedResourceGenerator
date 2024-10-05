namespace AssetAccessGenerator;

using System.Text;
using Microsoft.CodeAnalysis.CSharp;

internal static class Utils
{
	public static string GetRelativePath(string fullPath, string basePath)
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

	public static string GetResourceName(string resourcePath)
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

	public static string PathAsClassname(string path, string replacement = "")
	{
		return Utils.GetValidIdentifierName(path.Replace("\\", replacement).Replace("/", replacement));
	}

	public static string GetValidIdentifierName(string s) => SyntaxFacts.IsValidIdentifier(s) ? s : Utils.SanitizeIdentifierName(s);

	private static string SanitizeIdentifierName(string s)
	{
		ReadOnlySpan<char> span = s.AsSpan();

		if (span.IsEmpty)
		{
			return s;
		}

		Span<char> result = new char[s.Length].AsSpan();
		span.CopyTo(result);

		char firstChar = span[0];

		if (!SyntaxFacts.IsIdentifierStartCharacter(firstChar))
		{
			result[0] = '_';
		}

		for (var i = 1; i < span.Length; i++)
		{
			char current = span[i];
			if (!SyntaxFacts.IsIdentifierPartCharacter(current))
			{
				result[i] = '_';
			}
		}

		return result.ToString();
	}
}