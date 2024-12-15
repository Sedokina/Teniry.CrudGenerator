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
    private readonly EntityCustomizationSchemeFactory _sut;

    public EntityCustomizationSchemeFactoryTests()
    {
        _sut = new();
    }

    [Fact]
    public void Should_GetEntityTitleFromSymbol()
    {
        // Arrange
        var generator = CreateEntityGeneratorClass("Title = \"My custom title\";");

        // Act
        var actual = _sut.Construct(generator.Symbol, generator.Compilation);

        // Assert
        actual.Title.Should().Be("My custom title");
    }

    [Fact]
    public void Should_GetEntityPluralTitleFromSymbol()
    {
        // Arrange
        var generator = CreateEntityGeneratorClass("TitlePlural = \"My custom title plural\";");

        // Act
        var actual = _sut.Construct(generator.Symbol, generator.Compilation);

        // Assert
        actual.TitlePlural.Should().Be("My custom title plural");
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
        var actual = _sut.Construct(generator.Symbol, generator.Compilation);

        // Assert
        actual.DefaultSort.Should().NotBeNull();
        actual.DefaultSort!.Direction.Should().Be(direction);
        actual.DefaultSort.PropertyName.Should().Be(propertyName);
    }

    [Fact]
    public void Should_ConstructCreateOperationConfigurationWithAllProperties()
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
        var actual = _sut.Construct(generator.Symbol, generator.Compilation);

        // Assert
        actual.CreateOperation.Should().NotBeNull();
        actual.CreateOperation!.Generate.Should().BeTrue();
        actual.CreateOperation.Operation.Should().Be("CustomOperation");
        actual.CreateOperation.OperationGroup.Should().Be("ManagedEntityCreateOperationCustomNs");
        actual.CreateOperation.CommandName.Should().Be("CustomizedNameCreateManagedEntityCommand");
        actual.CreateOperation.HandlerName.Should().Be("CustomizedNameCreateManagedEntityHandler");
        actual.CreateOperation.DtoName.Should().Be("CustomizedNameCreatedManagedEntityDto");
        actual.CreateOperation.GenerateEndpoint.Should().BeTrue();
        actual.CreateOperation.EndpointClassName.Should().Be("CustomizedNameCreateManagedEntityEndpoint");
        actual.CreateOperation.EndpointFunctionName.Should().Be("RunCreateAsync");
        actual.CreateOperation.RouteName.Should().Be("/customizedManagedEntityCreate");
    }

    [Fact]
    public void Should_ConstructUpdateOperationConfigurationWithAllProperties()
    {
        // Arrange
        var generator = CreateEntityGeneratorClass(
            @"UpdateOperation = new EntityGeneratorUpdateOperationConfiguration
            {
                Generate = true,
                Operation = ""CustomOperation"",
                OperationGroup = ""ManagedEntityUpdateOperationCustomNs"",
                CommandName = ""CustomizedNameUpdateManagedEntityCommand"",
                HandlerName = ""CustomizedNameUpdateManagedEntityHandler"",
                GenerateEndpoint = true,
                EndpointClassName = ""CustomizedNameUpdateManagedEntityEndpoint"",
                EndpointFunctionName = ""RunUpdateAsync"",
                RouteName = ""/customizedManagedEntityUpdate""
                ViewModelName = ""CustomizedNameUpdatedManagedEntityViewModel""
            };"
        );

        // Act
        var actual = _sut.Construct(generator.Symbol, generator.Compilation);

        // Assert
        actual.UpdateOperation.Should().NotBeNull();
        actual.UpdateOperation!.Generate.Should().BeTrue();
        actual.UpdateOperation.Operation.Should().Be("CustomOperation");
        actual.UpdateOperation.OperationGroup.Should().Be("ManagedEntityUpdateOperationCustomNs");
        actual.UpdateOperation.CommandName.Should().Be("CustomizedNameUpdateManagedEntityCommand");
        actual.UpdateOperation.HandlerName.Should().Be("CustomizedNameUpdateManagedEntityHandler");
        actual.UpdateOperation.GenerateEndpoint.Should().BeTrue();
        actual.UpdateOperation.EndpointClassName.Should().Be("CustomizedNameUpdateManagedEntityEndpoint");
        actual.UpdateOperation.EndpointFunctionName.Should().Be("RunUpdateAsync");
        actual.UpdateOperation.RouteName.Should().Be("/customizedManagedEntityUpdate");
        actual.UpdateOperation.ViewModelName.Should().Be("CustomizedNameUpdatedManagedEntityViewModel");
    }

    [Fact]
    public void Should_ConstructDeleteOperationConfigurationWithAllProperties()
    {
        // Arrange
        var generator = CreateEntityGeneratorClass(
            @"DeleteOperation = new EntityGeneratorDeleteOperationConfiguration
            {
                Generate = true,
                Operation = ""CustomOperation"",
                OperationGroup = ""ManagedEntityDeleteOperationCustomNs"",
                CommandName = ""CustomizedNameDeleteManagedEntityCommand"",
                HandlerName = ""CustomizedNameDeleteManagedEntityHandler"",
                GenerateEndpoint = true,
                EndpointClassName = ""CustomizedNameDeleteManagedEntityEndpoint"",
                EndpointFunctionName = ""RunDeleteAsync"",
                RouteName = ""/customizedManagedEntityDelete""
            };"
        );

        // Act
        var actual = _sut.Construct(generator.Symbol, generator.Compilation);

        // Assert
        actual.DeleteOperation.Should().NotBeNull();
        actual.DeleteOperation!.Generate.Should().BeTrue();
        actual.DeleteOperation.Operation.Should().Be("CustomOperation");
        actual.DeleteOperation.OperationGroup.Should().Be("ManagedEntityDeleteOperationCustomNs");
        actual.DeleteOperation.CommandName.Should().Be("CustomizedNameDeleteManagedEntityCommand");
        actual.DeleteOperation.HandlerName.Should().Be("CustomizedNameDeleteManagedEntityHandler");
        actual.DeleteOperation.GenerateEndpoint.Should().BeTrue();
        actual.DeleteOperation.EndpointClassName.Should().Be("CustomizedNameDeleteManagedEntityEndpoint");
        actual.DeleteOperation.EndpointFunctionName.Should().Be("RunDeleteAsync");
        actual.DeleteOperation.RouteName.Should().Be("/customizedManagedEntityDelete");
    }

    [Fact]
    public void Should_ConstructGetByIdOperationConfigurationWithAllProperties()
    {
        // Arrange
        var generator = CreateEntityGeneratorClass(
            @"GetByIdOperation = new EntityGeneratorGetByIdOperationConfiguration
            {
                Generate = true,
                Operation = ""CustomOperation"",
                OperationGroup = ""ManagedEntityGetByIdOperationCustomNs"",
                QueryName = ""CustomizedNameGetByIdManagedEntityQuery"",
                DtoName = ""CustomizedNameGetByIddManagedEntityDto""
                HandlerName = ""CustomizedNameGetByIdManagedEntityHandler"",
                GenerateEndpoint = true,
                EndpointClassName = ""CustomizedNameGetByIdManagedEntityEndpoint"",
                EndpointFunctionName = ""RunGetByIdAsync"",
                RouteName = ""/customizedManagedEntityGetById""
            };"
        );

        // Act
        var actual = _sut.Construct(generator.Symbol, generator.Compilation);

        // Assert
        actual.GetByIdOperation.Should().NotBeNull();
        actual.GetByIdOperation!.Generate.Should().BeTrue();
        actual.GetByIdOperation.Operation.Should().Be("CustomOperation");
        actual.GetByIdOperation.OperationGroup.Should().Be("ManagedEntityGetByIdOperationCustomNs");
        actual.GetByIdOperation.QueryName.Should().Be("CustomizedNameGetByIdManagedEntityQuery");
        actual.GetByIdOperation.DtoName.Should().Be("CustomizedNameGetByIddManagedEntityDto");
        actual.GetByIdOperation.HandlerName.Should().Be("CustomizedNameGetByIdManagedEntityHandler");
        actual.GetByIdOperation.GenerateEndpoint.Should().BeTrue();
        actual.GetByIdOperation.EndpointClassName.Should().Be("CustomizedNameGetByIdManagedEntityEndpoint");
        actual.GetByIdOperation.EndpointFunctionName.Should().Be("RunGetByIdAsync");
        actual.GetByIdOperation.RouteName.Should().Be("/customizedManagedEntityGetById");
    }

    [Fact]
    public void Should_ConstructGetListOperationConfigurationWithAllProperties()
    {
        // Arrange
        var generator = CreateEntityGeneratorClass(
            @"GetListOperation = new EntityGeneratorGetListOperationConfiguration
            {
                Generate = true,
                Operation = ""CustomOperation"",
                OperationGroup = ""ManagedEntityGetListOperationCustomNs"",
                QueryName = ""CustomizedNameGetListManagedEntityQuery"",
                DtoName = ""CustomizedNameGetListManagedEntityDto""
                ListItemDtoName = ""CustomizedNameGetListItemManagedEntityDto""
                HandlerName = ""CustomizedNameGetListManagedEntityHandler"",
                GenerateEndpoint = true,
                EndpointClassName = ""CustomizedNameGetListManagedEntityEndpoint"",
                EndpointFunctionName = ""RunGetListAsync"",
                RouteName = ""/customizedManagedEntityGetList""
            };"
        );

        // Act
        var actual = _sut.Construct(generator.Symbol, generator.Compilation);

        // Assert
        actual.GetListOperation.Should().NotBeNull();
        actual.GetListOperation!.Generate.Should().BeTrue();
        actual.GetListOperation.Operation.Should().Be("CustomOperation");
        actual.GetListOperation.OperationGroup.Should().Be("ManagedEntityGetListOperationCustomNs");
        actual.GetListOperation.QueryName.Should().Be("CustomizedNameGetListManagedEntityQuery");
        actual.GetListOperation.DtoName.Should().Be("CustomizedNameGetListManagedEntityDto");
        actual.GetListOperation.ListItemDtoName.Should().Be("CustomizedNameGetListItemManagedEntityDto");
        actual.GetListOperation.HandlerName.Should().Be("CustomizedNameGetListManagedEntityHandler");
        actual.GetListOperation.GenerateEndpoint.Should().BeTrue();
        actual.GetListOperation.EndpointClassName.Should().Be("CustomizedNameGetListManagedEntityEndpoint");
        actual.GetListOperation.EndpointFunctionName.Should().Be("RunGetListAsync");
        actual.GetListOperation.RouteName.Should().Be("/customizedManagedEntityGetList");
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