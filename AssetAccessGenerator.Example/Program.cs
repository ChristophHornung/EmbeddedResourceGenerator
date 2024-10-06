namespace AssetAccessGenerator.Example;

internal class Program
{
	private static void Main()
	{
		Program.Enum(EmbeddedResource.TestAsset_Test_txt);
		Program.Enum(EmbeddedResource.TestAsset_Test2_txt);
		Program.Enum(EmbeddedResource.RootTest_txt);
		Program.EnumExtension(EmbeddedResource.RootTest_txt);
		Program.EnumExtension(Content.RootTest___content_txt);
		Program.Enum(Content.RootTest___content_txt);
	}

	private static void Enum(Content rootTestInclTxt)
	{
		Console.WriteLine(Contents.ReadAllText(rootTestInclTxt));
	}

	private static void EnumExtension(Content rootTestInclTxt)
	{
		Console.WriteLine(rootTestInclTxt.ReadAllText());
	}

	private static void Enum(EmbeddedResource resource)
	{
		using Stream s = EmbeddedResources.GetStream(resource);
		using StreamReader sr = new StreamReader(s);
		Console.WriteLine(sr.ReadToEnd());
	}

	private static void EnumExtension(EmbeddedResource resource)
	{
		using Stream s = resource.GetStream();
		using StreamReader sr = new StreamReader(s);
		Console.WriteLine(sr.ReadToEnd());
	}
}