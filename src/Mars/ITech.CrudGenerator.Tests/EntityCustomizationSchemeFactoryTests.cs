using System.Linq.Expressions;
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
    public void Should_GetEntityTitleFromSymbol()
    {
        // Arrange
        var generator = CreateEntityGeneratorClass("Title = \"My custom title\";");

        // Act
        var sut = _factory.Construct(generator.Symbol, generator.Compilation);

        // Assert
        sut.Title.Should().Be("My custom title");
    }

    [Fact]
    public void Should_GetEntityPluralTitleFromSymbol()
    {
        // Arrange
        var generator = CreateEntityGeneratorClass("TitlePlural = \"My custom title plural\";");

        // Act
        var sut = _factory.Construct(generator.Symbol, generator.Compilation);

        // Assert
        sut.TitlePlural.Should().Be("My custom title plural");
    }

    [Theory]
    [InlineData("asc", "Prop")]
    [InlineData("desc", "Prop")]
    public void Should_GetDefaultSortFromSymbol(string direction, string propertyName)
    {
        // Arrange
        var generator = CreateEntityGeneratorClass(
            $"DefaultSort = new EntityGeneratorDefaultSort<MyCustomClass>(\"{direction}\", x => x.{propertyName});");

        // Act
        var sut = _factory.Construct(generator.Symbol, generator.Compilation);

        // Assert
        sut.DefaultSort.Should().NotBeNull();
        sut.DefaultSort!.Direction.Should().Be(direction);
        sut.DefaultSort.PropertyName.Should().Be(propertyName);
    }

    [Fact]
    public void Should_GetEntityGeneratorCreateOperationConfigurationFromSymbol()
    {
        // Arrange
        var generator = CreateEntityGeneratorClass(
            @"CreateOperation = new EntityGeneratorCreateOperationConfiguration
            {
                Generate = true,
                Operation = ""CustomOperation"",
                OperationGroup = ""ManagedEntityCreateOperationCustomNs"",
                CommandName = ""CustomizedNameCreateManagedEntityCommand"",
                HandlerName = ""CustomizedNameCreateManagedEntityHandler"",
                DtoName = ""CustomizedNameCreatedManagedEntityDto"",
                GenerateEndpoint = true,
                EndpointClassName = ""CustomizedNameCreateManagedEntityEndpoint"",
                EndpointFunctionName = ""RunCreateAsync"",
                RouteName = ""/customizedManagedEntityCreate""
            };"
        );

        // Act
        var sut = _factory.Construct(generator.Symbol, generator.Compilation);

        // Assert
        sut.CreateOperation.Should().NotBeNull();
        sut.CreateOperation!.Generate.Should().BeTrue();
        sut.CreateOperation.Operation.Should().Be("CustomOperation");
        sut.CreateOperation.OperationGroup.Should().Be("ManagedEntityCreateOperationCustomNs");
        sut.CreateOperation.CommandName.Should().Be("CustomizedNameCreateManagedEntityCommand");
        sut.CreateOperation.HandlerName.Should().Be("CustomizedNameCreateManagedEntityHandler");
        sut.CreateOperation.DtoName.Should().Be("CustomizedNameCreatedManagedEntityDto");
        sut.CreateOperation.GenerateEndpoint.Should().BeTrue();
        sut.CreateOperation.EndpointClassName.Should().Be("CustomizedNameCreateManagedEntityEndpoint");
        sut.CreateOperation.EndpointFunctionName.Should().Be("RunCreateAsync");
        sut.CreateOperation.RouteName.Should().Be("/customizedManagedEntityCreate");
    }

    public (INamedTypeSymbol Symbol, CSharpCompilation Compilation) CreateEntityGeneratorClass(
        string ctorBody = "")
    {
        var className = "MyCustomClass";
        var entityClass = $@"using System;
using ITech.CrudGenerator.Abstractions.Configuration;

namespace ITech.CrudGenerator.Tests {{
    public class {className} {{
        public string Prop {{get; set;}}
    }}

    public class {className}Generator : EntityGeneratorConfiguration<{className}> {{
        public {className}Generator() {{
            {ctorBody}
        }}
    }}
}}
";

        var syntaxTree = CSharpSyntaxTree.ParseText(entityClass);
        var abstractions = MetadataReference.CreateFromFile(typeof(EntityGeneratorConfiguration<>).Assembly.Location);
        var linqExpression = MetadataReference.CreateFromFile(typeof(Expression<>).Assembly.Location);

        var references = GetMetadataReferencesFromDllNames([
            "System.Private.CoreLib.dll",
            "netstandard.dll",
            "System.Runtime.dll"
        ]);
        var compilation = CSharpCompilation.Create(
            Assembly.GetExecutingAssembly().FullName,
            [syntaxTree],
            references: [..references, abstractions, linqExpression],
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