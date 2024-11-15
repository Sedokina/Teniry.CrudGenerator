using System.Reflection;
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
        var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        var abstractions = MetadataReference.CreateFromFile(typeof(EntityGeneratorConfiguration<>).Assembly.Location);
        var compilation = CSharpCompilation.Create(
            Assembly.GetExecutingAssembly().FullName,
            [syntaxTree],
            references: new[] { mscorlib, abstractions },
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var erro = compilation.GetDiagnostics();
        var symbol = compilation.GetSymbolsWithName(className).First();
        return ((INamedTypeSymbol)symbol, compilation);
    }
}