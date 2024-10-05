﻿namespace ResourceAccessGenerator.Tests.Tests;

using System.Diagnostics;
using Xunit;

/// <summary>
/// The main tests. This file is deliberately not in the root folder to test that the generator works no
/// files in the csproj folder.
/// </summary>
public class EmbeddedResourceAccessGeneratorTests
{
	[Fact]
	public async Task IncludedTestTxtIsAccessible()
	{
		using var reader = AdditionalFile.TestIncludedAssets_IncludedTest_txt.GetReader();
		Assert.Equal("Success", await reader.ReadToEndAsync());

		var text = AdditionalFile.TestIncludedAssets_IncludedTest_txt.ReadAllText();
		Assert.Equal("Success", text);

		var text2 = await AdditionalFile.TestIncludedAssets_IncludedTest_txt.ReadAllTextAsync();
		Assert.Equal("Success", text2);

		var text3 = await AdditionalFile_TestIncludedAssets_OneMoreLevel.Test_txt.ReadAllTextAsync();
		Assert.Equal("Success", text3);

		using var reader3 = AdditionalFile_TestIncludedAssets.IncludedTest_txt.GetReader();
		Assert.Equal("Success", await reader3.ReadToEndAsync());
	}

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
	public void TestWithSpacesTxtIsAccessible()
	{
		using var reader = EmbeddedResource.TestAssets_Test_With_Spaces_txt.GetReader();
		Assert.Equal("Success", reader.ReadToEnd());

		using var reader2 = EmbeddedResources.TestAssets_Test_With_Spaces_txt_Reader;
		Assert.Equal("Success", reader2.ReadToEnd());

		using var reader3 = EmbeddedResourceTestAssets.Test_With_Spaces_txt.GetReader();
		Assert.Equal("Success", reader3.ReadToEnd());
	}

	[Fact]
	public void SubfolderTestWithSpacesTxtIsAccessible()
	{
		using var reader = EmbeddedResource.TestAssets_Subfolder_With_Spaces_Test_With_Spaces_txt.GetReader();
		Assert.Equal("Success", reader.ReadToEnd());

		using var reader2 = EmbeddedResources.TestAssets_Subfolder_With_Spaces_Test_With_Spaces_txt_Reader;
		Assert.Equal("Success", reader2.ReadToEnd());

		using var reader3 = EmbeddedResourceTestAssetsSubfolder_With_Spaces.Test_With_Spaces_txt.GetReader();
		Assert.Equal("Success", reader3.ReadToEnd());
	}

	[Fact]
	public void RootTestTxtIsAccessible()
	{
		using var reader = EmbeddedResource.TestAssets_Test_txt.GetReader();
		Assert.Equal("Success", reader.ReadToEnd());

		using var reader2 = EmbeddedResources.TestAssets_Test_txt_Reader;
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
	public void InvalidCharsSubfolderTxtIsAccessible()
	{
		typeof(EmbeddedResource).Assembly.GetManifestResourceNames().ToList().ForEach(s => Debug.WriteLine(s));
		using var reader = EmbeddedResource.TestAssets___InvalidChars_Test_txt.GetReader();
		Assert.Equal("Success", reader.ReadToEnd());

		using var reader2 = EmbeddedResources.TestAssets___InvalidChars_Test_txt_Reader;
		Assert.Equal("Success", reader2.ReadToEnd());

		using var reader3 = EmbeddedResourceTestAssets__InvalidChars.Test_txt.GetReader();
		Assert.Equal("Success", reader3.ReadToEnd());
	}

	[Fact]
	public void InvalidCharsRootTxtIsAccessible()
	{
		using var reader = EmbeddedResource.TestAssets_2InvalidChars___txt.GetReader();
		Assert.Equal("Success", reader.ReadToEnd());

		using var reader2 = EmbeddedResources.TestAssets___InvalidChars_Test_txt_Reader;
		Assert.Equal("Success", reader2.ReadToEnd());
	}
}