namespace EmbeddedResourceAccessGenerator.Tests;

using Xunit;

public class EmbeddedResourceAccessGeneratorTests
{
	[Fact]
	public void TestTxtIsAccessible()
	{
		using var reader = EmbeddedResource.TestAssets_Test_txt.GetReader();
		Assert.Equal("Success", reader.ReadToEnd());

		using var reader2 = EmbeddedResources.TestAssets_Test_txt_Reader;
		Assert.Equal("Success", reader2.ReadToEnd());
	}

	[Fact]
	public void RootTestTxtIsAccessible()
	{
		using var reader = EmbeddedResource.Test_txt.GetReader();
		Assert.Equal("Success", reader.ReadToEnd());

		using var reader2 = EmbeddedResources.Test_txt_Reader;
		Assert.Equal("Success", reader2.ReadToEnd());
	}

	[Fact]
	public void SubfolderTestTxtIsAccessible()
	{
		using var reader = EmbeddedResource.TestAssets_Subfolder_Test_txt.GetReader();
		Assert.Equal("Success", reader.ReadToEnd());

		using var reader2 = EmbeddedResources.TestAssets_Subfolder_Test_txt_Reader;
		Assert.Equal("Success", reader2.ReadToEnd());
	}

	[Fact]
	public void InvalidCharsTxtIsAccessible()
	{
		using var reader = EmbeddedResource.TestAssets_2InvalidChars___txt.GetReader();
		Assert.Equal("Success", reader.ReadToEnd());

		using var reader2 = EmbeddedResources.TestAssets_2InvalidChars___txt_Reader;
		Assert.Equal("Success", reader2.ReadToEnd());
	}

	[Fact]
	public void InvalidCharsRootTxtIsAccessible()
	{
		using var reader = EmbeddedResource._InvalidChars___txt.GetReader();
		Assert.Equal("Success", reader.ReadToEnd());

		using var reader2 = EmbeddedResources._InvalidChars___txt_Reader;
		Assert.Equal("Success", reader2.ReadToEnd());
	}
}