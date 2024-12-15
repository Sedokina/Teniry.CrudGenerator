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
        var sut = _factory.Construct(generator.Symbol, generator.Compilation);

        // Assert
        sut.UpdateOperation.Should().NotBeNull();
        sut.UpdateOperation!.Generate.Should().BeTrue();
        sut.UpdateOperation.Operation.Should().Be("CustomOperation");
        sut.UpdateOperation.OperationGroup.Should().Be("ManagedEntityUpdateOperationCustomNs");
        sut.UpdateOperation.CommandName.Should().Be("CustomizedNameUpdateManagedEntityCommand");
        sut.UpdateOperation.HandlerName.Should().Be("CustomizedNameUpdateManagedEntityHandler");
        sut.UpdateOperation.GenerateEndpoint.Should().BeTrue();
        sut.UpdateOperation.EndpointClassName.Should().Be("CustomizedNameUpdateManagedEntityEndpoint");
        sut.UpdateOperation.EndpointFunctionName.Should().Be("RunUpdateAsync");
        sut.UpdateOperation.RouteName.Should().Be("/customizedManagedEntityUpdate");
        sut.UpdateOperation.ViewModelName.Should().Be("CustomizedNameUpdatedManagedEntityViewModel");
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
        var sut = _factory.Construct(generator.Symbol, generator.Compilation);

        // Assert
        sut.DeleteOperation.Should().NotBeNull();
        sut.DeleteOperation!.Generate.Should().BeTrue();
        sut.DeleteOperation.Operation.Should().Be("CustomOperation");
        sut.DeleteOperation.OperationGroup.Should().Be("ManagedEntityDeleteOperationCustomNs");
        sut.DeleteOperation.CommandName.Should().Be("CustomizedNameDeleteManagedEntityCommand");
        sut.DeleteOperation.HandlerName.Should().Be("CustomizedNameDeleteManagedEntityHandler");
        sut.DeleteOperation.GenerateEndpoint.Should().BeTrue();
        sut.DeleteOperation.EndpointClassName.Should().Be("CustomizedNameDeleteManagedEntityEndpoint");
        sut.DeleteOperation.EndpointFunctionName.Should().Be("RunDeleteAsync");
        sut.DeleteOperation.RouteName.Should().Be("/customizedManagedEntityDelete");
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
        var sut = _factory.Construct(generator.Symbol, generator.Compilation);

        // Assert
        sut.GetByIdOperation.Should().NotBeNull();
        sut.GetByIdOperation!.Generate.Should().BeTrue();
        sut.GetByIdOperation.Operation.Should().Be("CustomOperation");
        sut.GetByIdOperation.OperationGroup.Should().Be("ManagedEntityGetByIdOperationCustomNs");
        sut.GetByIdOperation.QueryName.Should().Be("CustomizedNameGetByIdManagedEntityQuery");
        sut.GetByIdOperation.DtoName.Should().Be("CustomizedNameGetByIddManagedEntityDto");
        sut.GetByIdOperation.HandlerName.Should().Be("CustomizedNameGetByIdManagedEntityHandler");
        sut.GetByIdOperation.GenerateEndpoint.Should().BeTrue();
        sut.GetByIdOperation.EndpointClassName.Should().Be("CustomizedNameGetByIdManagedEntityEndpoint");
        sut.GetByIdOperation.EndpointFunctionName.Should().Be("RunGetByIdAsync");
        sut.GetByIdOperation.RouteName.Should().Be("/customizedManagedEntityGetById");
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
        var sut = _factory.Construct(generator.Symbol, generator.Compilation);

        // Assert
        sut.GetListOperation.Should().NotBeNull();
        sut.GetListOperation!.Generate.Should().BeTrue();
        sut.GetListOperation.Operation.Should().Be("CustomOperation");
        sut.GetListOperation.OperationGroup.Should().Be("ManagedEntityGetListOperationCustomNs");
        sut.GetListOperation.QueryName.Should().Be("CustomizedNameGetListManagedEntityQuery");
        sut.GetListOperation.DtoName.Should().Be("CustomizedNameGetListManagedEntityDto");
        sut.GetListOperation.ListItemDtoName.Should().Be("CustomizedNameGetListItemManagedEntityDto");
        sut.GetListOperation.HandlerName.Should().Be("CustomizedNameGetListManagedEntityHandler");
        sut.GetListOperation.GenerateEndpoint.Should().BeTrue();
        sut.GetListOperation.EndpointClassName.Should().Be("CustomizedNameGetListManagedEntityEndpoint");
        sut.GetListOperation.EndpointFunctionName.Should().Be("RunGetListAsync");
        sut.GetListOperation.RouteName.Should().Be("/customizedManagedEntityGetList");
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