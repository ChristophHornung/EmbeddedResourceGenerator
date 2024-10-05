namespace AssetAccessGenerator.Tests.Tests;

using Xunit;

public class ContentFilesTests
{
	[Fact]
	public async Task IncludedTestTxtIsAccessible()
	{
		using var reader = ContentFile.TestIncludedAssets_IncludedTest_txt.GetReader();
		Assert.Equal("Success", await reader.ReadToEndAsync());

		var text = ContentFile.TestIncludedAssets_IncludedTest_txt.ReadAllText();
		Assert.Equal("Success", text);

		var text2 = await ContentFile.TestIncludedAssets_IncludedTest_txt.ReadAllTextAsync(CancellationToken.None);
		Assert.Equal("Success", text2);

		var text3 = await ContentFile_TestIncludedAssets_OneMoreLevel.Test_txt.ReadAllTextAsync();
		Assert.Equal("Success", text3);

		using var reader3 = ContentFile_TestIncludedAssets.IncludedTest_txt.GetReader();
		Assert.Equal("Success", await reader3.ReadToEndAsync());
	}
}