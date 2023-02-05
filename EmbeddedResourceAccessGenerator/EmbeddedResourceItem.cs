namespace EmbeddedResourceAccessGenerator;

internal record EmbeddedResourceItem(string RelativePath, string IdentifierName, string ResourceName)
{
	public string RelativePath { get; } = RelativePath;
	public string ResourceName { get; } = ResourceName;
	public string IdentifierName { get; } = IdentifierName;
}