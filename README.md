[![StandWithUkraine](https://raw.githubusercontent.com/vshymanskyy/StandWithUkraine/main/badges/StandWithUkraine.svg)](https://github.com/vshymanskyy/StandWithUkraine/blob/main/docs/README.md)

# EmbeddedResourceAccessGenerator
[![NuGet version (Chorn.EmbeddedResourceAccessGenerator)](https://img.shields.io/nuget/v/Chorn.EmbeddedResourceAccessGenerator.svg?style=flat-square)](https://www.nuget.org/packages/Chorn.EmbeddedResourceAccessGenerator/)

The EmbeddedResourceAccessGenerator is a code generator to allow easy access to all
embedded resources.

## Usage
Get the nuget package [here](https://www.nuget.org/packages/Chorn.EmbeddedResourceAccessGenerator).

After referencing the `Chorn.EmbeddedResourceAccessGenerator` nuget the code generation will
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
    using Stream s = EmbeddedResource_TestAsset.Test_txt.GetStream();
    using StreamReader sr = EmbeddedResource_TestAsset.Test_txt.GetReader();
```

# ContentItemAccessGenerator
[![NuGet version (Chorn.ContentItemAccessGenerator)](https://img.shields.io/nuget/v/Chorn.ContentItemAccessGenerator.svg?style=flat-square)](https://www.nuget.org/packages/Chorn.ContentItemAccessGenerator/)

The ContentItemAccessGenerator is a code generator to allow easy access to all
content files with **CopyToOutputDirectory attribute specified as Always or PreserveNewest**.

## Usage
Get the nuget package [here](https://www.nuget.org/packages/Chorn.ContentItemAccessGenerator).

After referencing the `Chorn.ContentItemAccessGenerator` nuget the code generation will
automatically create a class `Contents` in the root namespace of the project.

Together with the generated `Content` enumeration there are several options to access
content files:

E.g. for a `Test.txt` embedded resource in the `TestAsset` folder:

- Via enum access through the `Content` enum:

```csharp
    // Via the generated extension methods on the enum
    using Stream s = Content.TestAsset_Test_txt.GetStream();
    using StreamReader sr = Content.TestAsset_Test_txt.GetReader();
```

- Via enum access through the `Content[FolderName]` enum:

```csharp
    // Via the generated extension methods on the enum
    using Stream s = Content_TestAsset.Test_txt.GetStream();
    using StreamReader sr = Content_TestAsset.Test_txt.GetReader();
```

# NoneItemAccessGenerator
[![NuGet version (Chorn.NoneItemAccessGenerator)](https://img.shields.io/nuget/v/Chorn.NoneItemAccessGenerator.svg?style=flat-square)](https://www.nuget.org/packages/Chorn.NoneItemAccessGenerator/)

The NoneItemAccessGenerator is a code generator to allow easy access to all
none item files with **CopyToOutputDirectory attribute specified as Always or PreserveNewest**.

## Usage
Get the nuget package [here](https://www.nuget.org/packages/Chorn.NoneItemAccessGenerator).

After referencing the `Chorn.NoneItemAccessGenerator` nuget the code generation will
automatically create a class `Nones` in the root namespace of the project.

Together with the generated `None` enumeration there are several options to access
none item files:

E.g. for a `Test.txt` embedded resource in the `TestAsset` folder:

- Via enum access through the `None` enum:

```csharp
    // Via the generated extension methods on the enum
    using Stream s = None.TestAsset_Test_txt.GetStream();
    using StreamReader sr = None.TestAsset_Test_txt.GetReader();
```

- Via enum access through the `None[FolderName]` enum:

```csharp
    // Via the generated extension methods on the enum
    using Stream s = None_TestAsset.Test_txt.GetStream();
    using StreamReader sr = None_TestAsset.Test_txt.GetReader();
```

## Motivation
Instead of using magic strings in the resource access code that may point to non-existant
resources this generator guarantees resources to exist and code to not compile when they are
removed.

Grouping the resources via their path adds path specific enums, e.g. to easily write tests
for all embedded resource in a subfolder.

Also it saves quite a bit of typing effort.
