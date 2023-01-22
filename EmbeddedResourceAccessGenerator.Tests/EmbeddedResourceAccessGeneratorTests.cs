namespace EmbeddedResourceAccessGenerator.Tests;

using Xunit;

public class EmbeddedResourceAccessGeneratorTests
{
	[Fact]
	public void TestTxtIsAccessible()
	{
		Assert.Equal("Success", EmbeddedResource.TestAssets_Test_txt.GetReader().ReadToEnd());
	}

	[Fact]
	public void RootTestTxtIsAccessible()
	{
		Assert.Equal("Success", EmbeddedResource.Test_txt.GetReader().ReadToEnd());
	}

	[Fact]
	public void SubfolderTestTxtIsAccessible()
	{
		Assert.Equal("Success", EmbeddedResource.TestAssets_Subfolder_Test_txt.GetReader().ReadToEnd());
	}
}