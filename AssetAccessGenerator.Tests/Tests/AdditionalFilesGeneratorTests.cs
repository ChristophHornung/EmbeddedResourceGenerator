namespace AssetAccessGenerator.Tests.Tests;

using Xunit;

public class AdditionalFilesGeneratorTests
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
}