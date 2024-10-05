namespace AssetAccessGenerator.Example;

internal class Program
{
	static void Main(string[] args)
	{
		Program.Direct();
		Program.Enum(EmbeddedResource.TestAsset_Test_txt);
		Program.Enum(EmbeddedResource.TestAsset_Test2_txt);
		Program.Enum(EmbeddedResource.RootTest_txt);
		Program.EnumExtension(EmbeddedResource.RootTest_txt);
		Program.EnumExtension(ContentFile.RootTest___content_txt);
		Program.Enum(ContentFile.RootTest___content_txt);
	}

	private static void Enum(ContentFile rootTestInclTxt)
	{
		Console.WriteLine(ContentFilesExtensions.ReadAllText(rootTestInclTxt));
	}

	private static void EnumExtension(ContentFile rootTestInclTxt)
	{
		Console.WriteLine(rootTestInclTxt.ReadAllText());
	}

	private static void Direct()
	{
		using Stream s = EmbeddedResources.TestAsset_Test_txt_Stream;
		using StreamReader sr = new StreamReader(s);
		Console.WriteLine(sr.ReadToEnd());
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