namespace AccessGenerator.Core;

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

	/// <summary>
	/// Returns true if <paramref name="path"/> starts with the path <paramref name="baseDirPath"/>.
	/// The comparison is case-insensitive, handles / and \ slashes as folder separators and
	/// only matches if the base dir folder name is matched exactly ("c:\foobar\file.txt" is not a sub path of "c:\foo").
	/// </summary>
	public static bool IsSubPathOf(this string path, string baseDirPath)
	{
		if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(baseDirPath))
		{
			return false;
		}

		var normalizedPath = Path.GetFullPath(path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar)
			.WithEnding(Path.DirectorySeparatorChar.ToString()));

		var normalizedBaseDirPath = Path.GetFullPath(baseDirPath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar)
			.WithEnding(Path.DirectorySeparatorChar.ToString()));

		return normalizedPath.StartsWith(normalizedBaseDirPath, StringComparison.OrdinalIgnoreCase);
	}

	/// <summary>
	/// Returns <paramref name="str"/> with the minimal concatenation of <paramref name="ending"/> (starting from end) that
	/// results in satisfying .EndsWith(ending).
	/// </summary>
	/// <example>"hel".WithEnding("llo") returns "hello", which is the result of "hel" + "lo".</example>
	private static string WithEnding(this string? str, string ending)
	{
		if (str == null)
			return ending;

		// Right() is 1-indexed, so include these cases
		// * Append no characters
		// * Append up to N characters, where N is ending length
		for (var i = 0; i <= ending.Length; i++)
		{
			var tmp = str + ending.Right(i);
			if (tmp.EndsWith(ending, StringComparison.OrdinalIgnoreCase))
				return tmp;
		}

		return str;
	}

	/// <summary>Gets the rightmost <paramref name="length" /> characters from a string.</summary>
	/// <param name="value">The string to retrieve the substring from.</param>
	/// <param name="length">The number of characters to retrieve.</param>
	/// <returns>The substring.</returns>
	private static string Right(this string value, int length)
	{
		if (value == null)
		{
			throw new ArgumentNullException(nameof(value));
		}
		if (length < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(length), length, "Length is less than zero");
		}

		return length < value.Length ? value.Substring(value.Length - length) : value;
	}

	public static string GetResourceName(string resourcePath)
	{
		// return CreateManifestResourceName.MakeValidEverettIdentifier(resourcePath);
		// If we reference Microsoft.Build.Tasks.Core we get this error on our referencing projects:
		// Error AAGEN001 Exception 'Could not load file or assembly 'Microsoft.Build.Tasks.Core, Version=15.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
		// or one of its dependencies. The system cannot find the file specified.'
		// at AssetAccessGenerator.AssetAccessGenerator.GetResourceName(String resourcePath)

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