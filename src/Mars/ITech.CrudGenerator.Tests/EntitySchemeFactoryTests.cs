using System.Reflection;
using ITech.CrudGenerator.Abstractions.DbContext;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.DbContext;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Core;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.EntityCustomization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ITech.CrudGenerator.Tests;

public class EntitySchemeFactoryTests
{
    private readonly EntitySchemeFactory _factory;
    private readonly EntityCustomizationScheme _entityCustomizationScheme;
    private readonly DbContextScheme _dbContextScheme;

    public EntitySchemeFactoryTests()
    {
        _factory = new();
        _entityCustomizationScheme = new();
        _dbContextScheme = new("", "", DbContextDbProvider.Mongo, new Dictionary<FilterType, FilterExpression>());
    }

    [Fact]
    public void Should_GetEntityNameFromSymbol()
    {
        // Arrange
        var symbol = GenerateEntity("MyEntityName");

        // Act
        var sut = _factory.Construct(symbol, _entityCustomizationScheme, _dbContextScheme);

        // Assert
        sut.EntityName.Name.Should().Be("MyEntityName");
    }

    [Theory]
    [InlineData("Currency", "Currencies")]
    [InlineData("Job", "Jobs")]
    [InlineData("Box", "Boxes")]
    [InlineData("DepartmentGroup", "DepartmentGroups")]
    public void Should_GeneratePluralName_From_EntityName(string singular, string plural)
    {
        // Arrange
        var symbol = GenerateEntity(singular);

        // Act
        var sut = _factory.Construct(symbol, _entityCustomizationScheme, _dbContextScheme);

        // Assert
        sut.EntityName.PluralName.Should().Be(plural);
    }

    [Theory]
    [InlineData("Series", "SeriesList")]
    [InlineData("Sheep", "SheepList")]
    [InlineData("Staff", "StaffList")]
    [InlineData("Employees", "EmployeesList")]
    [InlineData("Currencies", "CurrenciesList")]
    [InlineData("DepartmentGroups", "DepartmentGroupsList")]
    public void Should_addSuffixToEntityPluralName_When_PluralAndSingularFormIsSame(string singular, string plural)
    {
        // Arrange
        var symbol = GenerateEntity(singular);

        // Act
        var sut = _factory.Construct(symbol, _entityCustomizationScheme, _dbContextScheme);

        // Assert
        sut.EntityName.PluralName.Should().Be(plural);
    }

    [Fact]
    public void Should_GenerateTitle_From_EntityName()
    {
        // Arrange
        var symbol = GenerateEntity("MyEntityName");

        // Act
        var sut = _factory.Construct(symbol, _entityCustomizationScheme, _dbContextScheme);

        // Assert
        sut.EntityTitle.Title.Should().Be("My entity name");
    }

    [Theory]
    [InlineData("Currency", "Currencies")]
    [InlineData("DepartmentGroup", "Department groups")]
    [InlineData("Currencies", "Currencies list")]
    [InlineData("BooksGroups", "Books groups list")]
    public void Should_GeneratePluralTitle_From_EntityPluralName(string entityName, string pluralTitle)
    {
        // Arrange
        var symbol = GenerateEntity(entityName);

        // Act
        var sut = _factory.Construct(symbol, _entityCustomizationScheme, _dbContextScheme);

        // Assert
        sut.EntityTitle.PluralTitle.Should().Be(pluralTitle);
    }

    [Fact]
    public void Should_ExtractEntityNamespaceAndAssembly()
    {
        // Arrange
        var symbol = GenerateEntity("MyEntityName");

        // Act
        var sut = _factory.Construct(symbol, _entityCustomizationScheme, _dbContextScheme);

        // Assert
        sut.EntityNamespace.Should().Be("ITech.CrudGenerator.Tests");
        sut.ContainingAssembly.Should().Be(Assembly.GetExecutingAssembly().FullName);
    }

    [Fact]
    public void Should_NotHaveDefaultSort_When_DefaultSortNotSetInEntityCustomization()
    {
        // Arrange
        var symbol = GenerateEntity("MyEntityName");

        // Act
        var sut = _factory.Construct(symbol, _entityCustomizationScheme, _dbContextScheme);

        // Assert
        sut.DefaultSort.Should().BeNull();
    }

    [Fact]
    public void Should_UseDefaultSort_From_EntityCustomizationScheme()
    {
        // Arrange
        var symbol = GenerateEntity("MyEntityName");
        _entityCustomizationScheme.DefaultSort = new EntityDefaultSort("asc", "MyProp");

        // Act
        var sut = _factory.Construct(symbol, _entityCustomizationScheme, _dbContextScheme);

        // Assert
        sut.DefaultSort.Should().NotBeNull();
        sut.DefaultSort!.PropertyName.Should().Be("MyProp");
        sut.DefaultSort.Direction.Should().Be("asc");
    }

    [Fact]
    public void Should_IgnoreReferenceProperties()
    {
        // Arrange
        var symbol = GenerateEntity("MyEntityName", "public EntitySchemeFactoryTests ReferenceProp { get; set; }");

        // Act
        var sut = _factory.Construct(symbol, _entityCustomizationScheme, _dbContextScheme);

        // Assert
        sut.Properties.Should().NotContain(x => x.PropertyName == "ReferenceProp");
    }

    [Fact]
    public void Should_GetSimpleProperty()
    {
        // Arrange
        var symbol = GenerateEntity("MyEntityName", "public int SimpleProp { get; set; }");

        // Act
        var sut = _factory.Construct(symbol, _entityCustomizationScheme, _dbContextScheme);

        // Assert
        sut.Properties.Should().NotContain(x => x.PropertyName == "SimpleProp");
    }

    public ISymbol GenerateEntity(string entityName, string body = "")
    {
        var entityClass = $@"
using System;
namespace ITech.CrudGenerator.Tests;

public class {entityName}
{{
{body}
}}
";

        var syntaxTree = CSharpSyntaxTree.ParseText(entityClass);
        var compilation = CSharpCompilation.Create(Assembly.GetExecutingAssembly().FullName, [syntaxTree]);
        var symbol = compilation.GetSymbolsWithName(entityName).First();
        return symbol;
    }
}