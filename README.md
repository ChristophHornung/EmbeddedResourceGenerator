# AssetAccessGenerator
[![NuGet version (Chorn.AssetAccessGenerator)](https://img.shields.io/nuget/v/Chorn.AssetAccessGenerator.svg?style=flat-square)](https://www.nuget.org/packages/Chorn.AssetAccessGenerator/)


The AssetAccessGenerator is a code generator to allow easy access to all
embedded reaources and additional files.

## Usage
Get the nuget package [here](https://www.nuget.org/packages/Chorn.AssetAccessGenerator).

Add the GenerateEmbeddedResourceAcces attribute to the Embedded resource tag in your .csproj file:

```xml
<ItemGroup>
  <EmbeddedResource Include="TestAsset\Test.txt" GenerateEmbeddedResourceAccess="true"/>
</ItemGroup>
```

Or add the GenerateAdditionalFileAccess attribute to the AdditionalFiles tag in your .csproj file:

```xml
<ItemGroup>
    <AdditionalFiles Include="TestIncludedAssets\**\*.*" GenerateAdditionalFileAccess="true">
        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </AdditionalFiles>
</ItemGroup>
```

After referencing the `Chorn.AssetAccessGenerator` nuget the code generation will
automatically create a class `EmbeddedResources` and/or `AdditionalFilesExtensions` in the root namespace of the project.

Together with the generated `EmbeddedResource` and `AdditionalFile` enumeration there are several options to access
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

E.g. for a `Test.txt` additional file in the `TestAsset` folder:

- Via enum access through the `AdditionaFile` enum:

```csharp
	// Via the generated extension methods on the enum
	using Stream s = AdditionaFile.TestAsset_Test_txt.GetStream();
	using StreamReader sr = AdditionaFile.TestAsset_Test_txt.GetReader();
	var text = AdditionaFile.TestAsset_Test_txt.ReadAllText();
	var textAsync = await AdditionaFile.TestAsset_Test_txt.ReadAllTextAsync();
```

- Via enum access through the `AdditionaFile[FolderName]` enum:

```csharp
	// Via the generated extension methods on the enum
	using Stream s = AdditionaFile_FolderName.Test_txt.GetStream();
	using StreamReader sr = AdditionaFile_FolderName.Test_txt.GetReader();
	var text = AdditionaFile_FolderName.Test_txt.ReadAllText();
	var textAsync = await AdditionaFile_FolderName.Test_txt.ReadAllTextAsync();
```

## Motivation
Instead of using magic strings in the resource access code that may point to non-existant
resources this generator guarantees resources to exist and code to not compile when they are
removed.

Grouping the resources via their path adds path specific enums, e.g. to easily write tests
for all embedded resource in a subfolder.

Also it saves quite a bit of typing effort.
