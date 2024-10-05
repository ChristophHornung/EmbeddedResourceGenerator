# ResourceAccessGenerator
[![NuGet version (Chorn.AssetAccessGenerator)](https://img.shields.io/nuget/v/Chorn.ResourceAccessGenerator.svg?style=flat-square)](https://www.nuget.org/packages/Chorn.ResourceAccessGenerator/)


The AssetAccessGenerator is a code generator to allow easy access to all
embedded and additional resources.

## Usage
Get the nuget package [here](https://www.nuget.org/packages/Chorn.AssetAccessGenerator).

After referencing the `Chorn.AssetAccessGenerator` nuget the code generation will
automatically create a class `EmbeddedResources` in the root namespace of the project.

Together with the generated `EmbeddedResource` enumeration there are several options to access
embedded resources:

E.g. for a `Test.txt` embedded resource in the `TestAsset` folder:

- Via enum access through the `EmbeddedResource` enum:

```csharp
	// Via the generated extension methods on the enum
	using Stream s = EmbeddedResource.TestAsset_Test_txt.GetStream();
	using StreamReader sr = EmbeddedResource.TestAsset_Test_txt.GetReader();
```

- Via enum access through the `EmbeddedResource[FolderName]` enum:

```csharp
	// Via the generated extension methods on the enum
	using Stream s = EmbeddedResourceTestAsset.Test_txt.GetStream();
	using StreamReader sr = EmbeddedResourceTestAsset.Test_txt.GetReader();
```

## Motivation
Instead of using magic strings in the resource access code that may point to non-existant
resources this generator guarantees resources to exist and code to not compile when they are
removed.

Grouping the resources via their path adds path specific enums, e.g. to easily write tests
for all embedded resource in a subfolder.

Also it saves quite a bit of typing effort.
