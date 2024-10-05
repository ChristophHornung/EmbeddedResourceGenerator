namespace ResourceAccessGenerator;

using System.Globalization;
using System.Text;

internal static class EverettIdentifierHelper
{
	/// <summary>
	/// This method is provided for compatibility with Everett which used to convert parts of resource names into
	/// valid identifiers
	/// </summary>
	public static string MakeValidEverettIdentifier(string? name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return string.Empty;
		}

		var everettId = new StringBuilder(name!.Length);

		// split the name into folder names
		string[] subNames = name.Split('\\', '/');

		// convert every folder name
		EverettIdentifierHelper.MakeValidEverettFolderIdentifier(everettId, subNames[0]);

		for (int i = 1; i < subNames.Length; i++)
		{
			everettId.Append('.');
			EverettIdentifierHelper.MakeValidEverettFolderIdentifier(everettId, subNames[i]);
		}

		return everettId.ToString();
	}

	/// <summary>
	/// Is the character a valid first Everett identifier character?
	/// </summary>
	private static bool IsValidEverettIdFirstChar(char c)
	{
		return
			char.IsLetter(c) ||
			CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.ConnectorPunctuation;
	}

	/// <summary>
	/// Is the character a valid Everett identifier character?
	/// </summary>
	private static bool IsValidEverettIdChar(char c)
	{
		UnicodeCategory cat = CharUnicodeInfo.GetUnicodeCategory(c);

		return
			char.IsLetterOrDigit(c) ||
			cat is UnicodeCategory.ConnectorPunctuation
				or UnicodeCategory.NonSpacingMark
				or UnicodeCategory.SpacingCombiningMark
				or UnicodeCategory.EnclosingMark;
	}

	/// <summary>
	/// Make a folder name into an Everett-compatible identifier
	/// </summary>
	private static void MakeValidEverettFolderIdentifier(StringBuilder builder, string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return;
		}

		// store the original length for use later
		int length = builder.Length;

		// split folder name into subfolder names separated by '.', if any
		string[] subNames = name.Split('.');

		// convert each subfolder name separately
		EverettIdentifierHelper.MakeValidEverettSubFolderIdentifier(builder, subNames[0]);

		for (int i = 1; i < subNames.Length; i++)
		{
			builder.Append('.');
			EverettIdentifierHelper.MakeValidEverettSubFolderIdentifier(builder, subNames[i]);
		}

		// folder name cannot be a single underscore - add another underscore to it
		if ((builder.Length - length) == 1 && builder[length] == '_')
		{
			builder.Append('_');
		}
	}

	/// <summary>
	/// Make a folder subfolder name into an Everett-compatible identifier 
	/// </summary>
	private static void MakeValidEverettSubFolderIdentifier(StringBuilder builder, string subName)
	{
		if (string.IsNullOrEmpty(subName))
		{
			return;
		}

		// the first character has stronger restrictions than the rest
		if (EverettIdentifierHelper.IsValidEverettIdFirstChar(subName[0]))
		{
			builder.Append(subName[0]);
		}
		else
		{
			builder.Append('_');
			if (EverettIdentifierHelper.IsValidEverettIdChar(subName[0]))
			{
				// if it is a valid subsequent character, prepend an underscore to it
				builder.Append(subName[0]);
			}
		}

		// process the rest of the subfolder name
		for (int i = 1; i < subName.Length; i++)
		{
			builder.Append(EverettIdentifierHelper.IsValidEverettIdChar(subName[i]) ? subName[i] : '_');
		}
	}
}