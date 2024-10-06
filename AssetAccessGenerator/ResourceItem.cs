namespace AssetAccessGenerator;

using System.Collections;
using System.Collections.Immutable;

/// <summary>
/// Represents a resource item.
/// </summary>
public record ResourceItem(
    string RelativePath,
    string IdentifierName,
    string ResourceName,
    ResourceKind ResourceKind)
{
    /// <summary>
    /// Gets the relative path of the resource item.
    /// </summary>
    public string RelativePath { get; } = RelativePath;

    /// <summary>
    /// Gets the name of the resource.
    /// </summary>
    public string ResourceName { get; } = ResourceName;

    /// <summary>
    /// Gets the identifier name of the resource item.
    /// </summary>
    public string IdentifierName { get; } = IdentifierName;

    /// <summary>
    /// Gets the kind of the resource.
    /// </summary>
    public ResourceKind ResourceKind { get; } = ResourceKind;
}

/// <summary>
/// Represents the kind of resource.
/// </summary>
public enum ResourceKind
{
    Unspecified,
    EmbeddedResource,
    Content,
    None
}

/// <summary>
/// Represents the context for resource generation.
/// </summary>
public record GenerationContext(ImmutableArray<ResourceItem> Resources, string RootNamespace) : IEnumerable<ResourceItem>
{
    private ImmutableArray<ResourceItem> Resources = Resources;

    /// <summary>
    /// Gets the root namespace.
    /// </summary>
    public string RootNamespace { get; } = RootNamespace;

    /// <summary>
    /// Creates a new resource generation context with the specified resource kind.
    /// </summary>
    /// <param name="kind">The resource kind.</param>
    /// <returns>A new resource generation context.</returns>
    public GenerationContext With(ResourceKind kind)
    {
        return this with { Resources = this.Resources.Where(x => x.ResourceKind == kind).ToImmutableArray() };
    }

    /// <inheritdoc />
    public IEnumerator<ResourceItem> GetEnumerator()
    {
        return this.Resources.AsEnumerable().GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    /// <summary>
    /// Gets a value indicating whether the resource generation context is empty.
    /// </summary>
    public bool IsEmpty => this.Resources.IsEmpty;
}
