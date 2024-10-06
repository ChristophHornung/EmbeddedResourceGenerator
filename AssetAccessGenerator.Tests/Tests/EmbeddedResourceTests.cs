namespace AssetAccessGenerator.Tests.Tests;

using System.Diagnostics;
using Xunit;

/// <summary>
/// The main tests. This file is deliberately not in the root folder to test that the generator works no
/// files in the csproj folder.
/// </summary>
public class EmbeddedAssetAccessGeneratorTests
{
	[Fact]
	public void TestTxtIsAccessible()
	{
		using var reader = EmbeddedResource.TestAssets_Test_txt.GetReader();
		Assert.Equal("Success", reader.ReadToEnd());

		using var reader2 = EmbeddedResource_TestAssets.Test_txt.GetReader();
		Assert.Equal("Success", reader2.ReadToEnd());
	}

	[Fact]
	public void TestWithSpacesTxtIsAccessible()
	{
		using var reader = EmbeddedResource.TestAssets_Test_With_Spaces_txt.GetReader();
		Assert.Equal("Success", reader.ReadToEnd());

		using var reader2 = EmbeddedResource_TestAssets.Test_With_Spaces_txt.GetReader();
		Assert.Equal("Success", reader2.ReadToEnd());
	}

	[Fact]
	public void SubfolderTestWithSpacesTxtIsAccessible()
	{
		using var reader = EmbeddedResource.TestAssets_Subfolder_With_Spaces_Test_With_Spaces_txt.GetReader();
		Assert.Equal("Success", reader.ReadToEnd());

		using var reader2 = EmbeddedResource_TestAssets_Subfolder_With_Spaces.Test_With_Spaces_txt.GetReader();
		Assert.Equal("Success", reader2.ReadToEnd());
	}

	[Fact]
	public void RootTestTxtIsAccessible()
	{
		using var reader = EmbeddedResource.TestAssets_Test_txt.GetReader();
		Assert.Equal("Success", reader.ReadToEnd());
	}

	[Fact]
	public void SubfolderTestTxtIsAccessible()
	{
		using var reader = EmbeddedResource.TestAssets_Subfolder_Test_txt.GetReader();
		Assert.Equal("Success", reader.ReadToEnd());

		using var reader2 = EmbeddedResource_TestAssets_Subfolder.Test_txt.GetReader();
		Assert.Equal("Success", reader2.ReadToEnd());
	}

	[Fact]
	public void InvalidCharsTxtIsAccessible()
	{
		using var reader = EmbeddedResource.TestAssets_2InvalidChars___txt.GetReader();
		Assert.Equal("Success", reader.ReadToEnd());

		var text = EmbeddedResource.TestAssets_2InvalidChars___txt.ReadAllText();
		Assert.Equal("Success", text);

		using var reader2 = EmbeddedResource_TestAssets._InvalidChars___txt.GetReader();
		Assert.Equal("Success", reader2.ReadToEnd());
	}

	[Fact]
	public void InvalidCharsSubfolderTxtIsAccessible()
	{
		typeof(EmbeddedResource).Assembly.GetManifestResourceNames().ToList().ForEach(s => Debug.WriteLine(s));
		using var reader = EmbeddedResource.TestAssets___InvalidChars_Test_txt.GetReader();
		Assert.Equal("Success", reader.ReadToEnd());

		using var reader2 = EmbeddedResource_TestAssets___InvalidChars.Test_txt.GetReader();
		Assert.Equal("Success", reader2.ReadToEnd());
	}

	[Fact]
	public void InvalidCharsRootTxtIsAccessible()
	{
		using var reader = EmbeddedResource._InvalidChars___txt.GetReader();
		Assert.Equal("Success", reader.ReadToEnd());
	}
}