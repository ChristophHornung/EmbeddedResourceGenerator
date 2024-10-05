namespace AssetAccessGenerator.Tests.Tests;

using Xunit;

public class ContentFilesTests
{
	[Fact]
	public async Task ContentTestTxtIsAccessible()
	{
		using var reader = ContentFile.TestContentAssets_ContentTest_txt.GetReader();
		Assert.Equal("Success", await reader.ReadToEndAsync());

		var text = ContentFile.TestContentAssets_ContentTest_txt.ReadAllText();
		Assert.Equal("Success", text);

		var text2 = await ContentFile.TestContentAssets_ContentTest_txt.ReadAllTextAsync(CancellationToken.None);
		Assert.Equal("Success", text2);

		var text3 = await ContentFile_TestContentAssets_OneMoreLevel.Test_txt.ReadAllTextAsync();
		Assert.Equal("Success", text3);

		using var reader3 = ContentFile_TestContentAssets.ContentTest_txt.GetReader();
		Assert.Equal("Success", await reader3.ReadToEndAsync());
	}
}