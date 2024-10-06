namespace AssetAccessGenerator.Tests.Tests;

using Xunit;

public class NoneFilesTests
{
	[Fact]
	public async Task NoneItemTestTxtIsAccessible()
	{
		using var reader = None.TestNoneAssets_NoneTest_txt.GetReader();
		Assert.Equal("Success", await reader.ReadToEndAsync());

		var text = None.TestNoneAssets_NoneTest_txt.ReadAllText();
		Assert.Equal("Success", text);

		var text2 = await None.TestNoneAssets_NoneTest_txt.ReadAllTextAsync(CancellationToken.None);
		Assert.Equal("Success", text2);
	}
}