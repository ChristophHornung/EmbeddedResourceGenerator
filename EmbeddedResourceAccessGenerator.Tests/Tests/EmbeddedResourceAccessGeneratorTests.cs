namespace EmbeddedResourceAccessGenerator.Tests.Tests;

using Xunit;

/// <summary>
/// The main tests. This file is deliberately not in the root folder to test that the generator works no
/// files in the csproj folder.
/// </summary>
public class EmbeddedResourceAccessGeneratorTests
{
	[Fact]
	public void TestTxtIsAccessible()
	{
		using var reader = EmbeddedResource.TestAssets_Test_txt.GetReader();
		Assert.Equal("Success", reader.ReadToEnd());

		using var reader2 = EmbeddedResources.TestAssets_Test_txt_Reader;
		Assert.Equal("Success", reader2.ReadToEnd());

		using var reader3 = EmbeddedResourceTestAssets.Test_txt.GetReader();
		Assert.Equal("Success", reader3.ReadToEnd());
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

		using var reader3 = EmbeddedResourceTestAssetsSubfolder.Test_txt.GetReader();
		Assert.Equal("Success", reader3.ReadToEnd());
	}

	[Fact]
	public void InvalidCharsTxtIsAccessible()
	{
		using var reader = EmbeddedResource.TestAssets_2InvalidChars___txt.GetReader();
		Assert.Equal("Success", reader.ReadToEnd());

		using var reader2 = EmbeddedResources.TestAssets_2InvalidChars___txt_Reader;
		Assert.Equal("Success", reader2.ReadToEnd());

		using var reader3 = EmbeddedResourceTestAssets._InvalidChars___txt.GetReader();
		Assert.Equal("Success", reader3.ReadToEnd());
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