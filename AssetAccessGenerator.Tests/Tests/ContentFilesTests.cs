namespace AssetAccessGenerator.Tests.Tests;

using Xunit;

public class ContentFilesTests
{
	[Fact]
	[Fact]
	public async Task ContentTestTxtIsAccessible()
	{
		using var reader = Content.TestContentAssets_ContentTest_txt.GetReader();
		Assert.Equal("Success", await reader.ReadToEndAsync());

		var text = Content.TestContentAssets_ContentTest_txt.ReadAllText();
		Assert.Equal("Success", text);

		var text2 = await Content.TestContentAssets_ContentTest_txt.ReadAllTextAsync(CancellationToken.None);
		Assert.Equal("Success", text2);

		var text3 = await Content_TestContentAssets_OneMoreLevel.Test_txt.ReadAllTextAsync();
		Assert.Equal("Success", text3);

		var bytes1 = await Content.TestContentAssets_ContentTest_txt.ReadAllBytesAsync(CancellationToken.None);

		Assert.Equivalent(new byte[] { 239, 187, 191, 83, 117, 99, 99, 101, 115, 115 }, bytes1);

		var bytes2 = Content.TestContentAssets_ContentTest_txt.ReadAllBytes();

		Assert.Equivalent(new byte[] { 239, 187, 191, 83, 117, 99, 99, 101, 115, 115 }, bytes2);

		using var reader3 = Content_TestContentAssets.ContentTest_txt.GetReader();
		Assert.Equal("Success", await reader3.ReadToEndAsync());
	}
}