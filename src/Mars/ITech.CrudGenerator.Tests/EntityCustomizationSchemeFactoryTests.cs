using System.Reflection;
using System.Runtime.InteropServices;
using ITech.CrudGenerator.Abstractions.Configuration;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.EntityCustomization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ITech.CrudGenerator.Tests;

public class EntityCustomizationSchemeFactoryTests
{
    private readonly EntityCustomizationSchemeFactory _factory;

    public EntityCustomizationSchemeFactoryTests()
    {
        _factory = new();
    }

    [Fact]
    public void Should_GetEntityNameFromSymbol()
    {
        // Arrange
        var generator = CreateEntityGeneratorClass("Title = \"My custom title\";");

        // Act
        var sut = _factory.Construct(generator.Symbol, generator.Compilation);

        // Assert
        sut.Title.Should().Be("My custom title");
    }

    public (INamedTypeSymbol Symbol, CSharpCompilation Compilation) CreateEntityGeneratorClass(
        string ctorBody = "")
    {
        var className = "MyCustomClass";
        var entityClass = $@"using System;
using ITech.CrudGenerator.Abstractions.Configuration;

namespace ITech.CrudGenerator.Tests {{
    public class {className} {{ }}

    public class {className}Generator : EntityGeneratorConfiguration<{className}> {{
        public {className}Generator() {{
            {ctorBody}
        }}
    }}
}}
";

        var syntaxTree = CSharpSyntaxTree.ParseText(entityClass);
        var abstractions = MetadataReference.CreateFromFile(typeof(EntityGeneratorConfiguration<>).Assembly.Location);

        var references = GetMetadataReferencesFromDllNames([
            "System.Private.CoreLib.dll",
            "netstandard.dll",
            "System.Runtime.dll"
        ]);
        var compilation = CSharpCompilation.Create(
            Assembly.GetExecutingAssembly().FullName,
            [syntaxTree],
            references: [..references, abstractions],
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var err = compilation.GetDiagnostics();
        var symbol = compilation.GetSymbolsWithName($"{className}Generator").First();
        return ((INamedTypeSymbol)symbol, compilation);
    }

    private PortableExecutableReference[] GetMetadataReferencesFromDllNames(string[] dllNames)
    {
        var runtimeDirectory = RuntimeEnvironment.GetRuntimeDirectory();
        return dllNames
            .Select(x => MetadataReference.CreateFromFile(Path.Combine(runtimeDirectory, x)))
            .ToArray();
    }
}