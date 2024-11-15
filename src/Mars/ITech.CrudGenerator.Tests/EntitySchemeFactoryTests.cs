using System.Reflection;
using ITech.CrudGenerator.Abstractions.DbContext;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.DbContext;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Core;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Expressions;
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
        _dbContextScheme = new("", "", DbContextDbProvider.Mongo, new Dictionary<FilterType, FilterExpression>()
        {
            { FilterType.Contains, new ContainsFilterExpression() },
            { FilterType.Equals, new EqualsFilterExpression() },
            { FilterType.GreaterThanOrEqual, new GreaterThanOrEqualFilterExpression() },
            { FilterType.LessThan, new LessThanFilterExpression() },
            { FilterType.Like, new LikeFilterExpression() },
        });
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
    public void Should_ExtractSimpleProperty()
    {
        // Arrange
        var symbol = GenerateEntity("MyEntityName", "public int SimpleProp { get; set; }");

        // Act
        var sut = _factory.Construct(symbol, _entityCustomizationScheme, _dbContextScheme);

        // Assert
        sut.Properties.Should().Contain(x => x.PropertyName == "SimpleProp");
    }

    [Fact]
    public void Should_IgnoreSystemPrefixInPropertyTypes()
    {
        // Arrange
        var symbol = GenerateEntity("MyEntityName", "public DateTimeOffset DateTimeProp { get; set; }");

        // Act
        var sut = _factory.Construct(symbol, _entityCustomizationScheme, _dbContextScheme);

        // Assert
        sut.Properties.Should().Contain(x => x.PropertyName == "DateTimeProp" && x.TypeName.Equals("DateTimeOffset"));
    }

    [Fact]
    public void Should_HaveDefaultValueForString()
    {
        // Arrange
        var symbol = GenerateEntity("MyEntityName", "public string StringProp { get; set; }");

        // Act
        var sut = _factory.Construct(symbol, _entityCustomizationScheme, _dbContextScheme);

        // Assert
        sut.Properties.Should().Contain(x => x.PropertyName == "StringProp" &&
                                             x.DefaultValue != null &&
                                             x.DefaultValue.Equals("\"\""));
    }

    [Theory]
    [InlineData("MyEntityNameId", true)]
    [InlineData("Id", true)]
    [InlineData("_id", true)]
    [InlineData("OtherEntityId", false)]
    [InlineData("NotRelatedProperty", false)]
    public void Should_DetectEntityIdProperty(string propertyName, bool isEntityId)
    {
        // Arrange
        var symbol = GenerateEntity("MyEntityName", $"public Guid {propertyName} {{ get; set; }}");

        // Act
        var sut = _factory.Construct(symbol, _entityCustomizationScheme, _dbContextScheme);

        // Assert
        sut.Properties.Should().Contain(x => x.PropertyName == propertyName &&
                                             x.IsEntityId == isEntityId);
    }

    [Theory]
    [InlineData("bool")]
    [InlineData("sbyte")]
    [InlineData("byte")]
    [InlineData("short")]
    [InlineData("int")]
    [InlineData("long")]
    [InlineData("ushort")]
    [InlineData("uint")]
    [InlineData("ulong")]
    [InlineData("float")]
    [InlineData("double")]
    [InlineData("decimal")]
    [InlineData("char")]
    [InlineData("string")]
    [InlineData("DateTime")]
    [InlineData("DateTimeOffset")]
    [InlineData("Guid")]
    public void Should_DetectSortableProperty(string propertyType)
    {
        // Arrange
        var symbol = GenerateEntity("MyEntityName", $"public {propertyType} SortableProperty {{ get; set; }}");

        // Act
        var sut = _factory.Construct(symbol, _entityCustomizationScheme, _dbContextScheme);

        // Assert
        sut.Properties.Should().Contain(x => x.PropertyName == "SortableProperty" && x.CanBeSorted == true);
    }

    [Fact]
    public void Should_NameSortablePropertyKeyInCamelCase()
    {
        // Arrange
        var symbol = GenerateEntity("MyEntityName", "public int SortableProperty {{ get; set; }}");

        // Act
        var sut = _factory.Construct(symbol, _entityCustomizationScheme, _dbContextScheme);

        // Assert
        sut.Properties.Should()
            .Contain(x => x.PropertyName == "SortableProperty" && x.SortKey.Equals("sortableProperty"));
    }

    [Fact]
    public void Should_HaveContainsFilter_ForGuidProperty()
    {
        // Arrange
        var symbol = GenerateEntity("MyEntityName", "public Guid FilteredProperty {{ get; set; }}");

        // Act
        var sut = _factory.Construct(symbol, _entityCustomizationScheme, _dbContextScheme);

        // Assert
        sut.Properties.Should()
            .SatisfyRespectively(x =>
            {
                x.PropertyName.Should().Be("FilteredProperty");
                x.FilterProperties.Should()
                    .HaveCount(1)
                    .And
                    .AllSatisfy(f =>
                    {
                        f.PropertyName.Should().Be("FilteredProperty");
                        f.TypeName.Should().Be("Guid[]");
                        f.FilterExpression.Should().BeOfType<ContainsFilterExpression>();
                    });
            });
    }

    [Theory]
    [InlineData("int", "MyEntityNameId", "MyEntityNameIds")]
    [InlineData("int", "Id", "Ids")]
    [InlineData("int", "_id", "_ids")]
    [InlineData("int", "OtherEntityId", "OtherEntityIds")]
    [InlineData("int", "OtherEntity_id", "OtherEntity_ids")]
    [InlineData("Guid", "MyEntityNameId", "MyEntityNameIds")]
    [InlineData("Guid", "Id", "Ids")]
    [InlineData("Guid", "_id", "_ids")]
    [InlineData("Guid", "OtherEntityId", "OtherEntityIds")]
    [InlineData("Guid", "OtherEntity_id", "OtherEntity_ids")]
    public void Should_HaveContainsFilter_ForPrimaryOrForeignKey(
        string typeName,
        string propertyName,
        string expectedFilterPropertyName)
    {
        // Arrange
        var symbol = GenerateEntity("MyEntityName", $"public {typeName} {propertyName} {{ get; set; }}");

        // Act
        var sut = _factory.Construct(symbol, _entityCustomizationScheme, _dbContextScheme);

        // Assert
        sut.Properties.Should()
            .SatisfyRespectively(x =>
            {
                x.PropertyName.Should().Be(propertyName);
                x.FilterProperties.Should()
                    .HaveCount(1)
                    .And
                    .AllSatisfy(f =>
                    {
                        f.PropertyName.Should().Be(expectedFilterPropertyName);
                        f.TypeName.Should().Be($"{typeName}[]?");
                        f.FilterExpression.Should().BeOfType<ContainsFilterExpression>();
                    });
            });
    }

    [Theory]
    [InlineData("sbyte")]
    [InlineData("byte")]
    [InlineData("short")]
    [InlineData("int")]
    [InlineData("long")]
    [InlineData("ushort")]
    [InlineData("uint")]
    [InlineData("ulong")]
    [InlineData("float")]
    [InlineData("double")]
    [InlineData("decimal")]
    [InlineData("DateTime")]
    [InlineData("DateTimeOffset")]
    public void Should_HaveRangeFilter_ForSimpleTypes(string typeName)
    {
        // Arrange
        var propertyName = "FilteredProperty";
        var symbol = GenerateEntity("MyEntityName", $"public {typeName} {propertyName} {{ get; set; }}");

        // Act
        var sut = _factory.Construct(symbol, _entityCustomizationScheme, _dbContextScheme);

        // Assert
        sut.Properties.Should()
            .SatisfyRespectively(x =>
            {
                x.PropertyName.Should().Be(propertyName);
                x.FilterProperties.Should()
                    .HaveCount(2)
                    .And
                    .SatisfyRespectively(f =>
                    {
                        f.PropertyName.Should().Be($"{propertyName}From");
                        f.TypeName.Should().Be($"{typeName}?");
                        f.FilterExpression.Should().BeOfType<GreaterThanOrEqualFilterExpression>();
                    }, f =>
                    {
                        f.PropertyName.Should().Be($"{propertyName}To");
                        f.TypeName.Should().Be($"{typeName}?");
                        f.FilterExpression.Should().BeOfType<LessThanFilterExpression>();
                    });
            });
    }

    [Fact]
    public void Should_HaveLikeFilter_ForStringType()
    {
        // Arrange
        var propertyName = "FilteredProperty";
        var typeName = "string";
        var symbol = GenerateEntity("MyEntityName", $"public {typeName} {propertyName} {{ get; set; }}");

        // Act
        var sut = _factory.Construct(symbol, _entityCustomizationScheme, _dbContextScheme);

        // Assert
        sut.Properties.Should()
            .SatisfyRespectively(x =>
            {
                x.PropertyName.Should().Be(propertyName);
                x.FilterProperties.Should()
                    .HaveCount(1)
                    .And
                    .SatisfyRespectively(f =>
                    {
                        f.PropertyName.Should().Be(propertyName);
                        f.TypeName.Should().Be($"{typeName}?");
                        f.FilterExpression.Should().BeOfType<LikeFilterExpression>();
                    });
            });
    }

    public ISymbol GenerateEntity(string entityName, string body = "")
    {
        var entityClass = $@"using System;

namespace ITech.CrudGenerator.Tests {{
    public class {entityName}
    {{
        {body}
    }}
}}
";

        var syntaxTree = CSharpSyntaxTree.ParseText(entityClass);
        var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        var compilation = CSharpCompilation.Create(
            Assembly.GetExecutingAssembly().FullName,
            [syntaxTree],
            references: new[] { mscorlib },
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var symbol = compilation.GetSymbolsWithName(entityName).First();
        return symbol;
    }
}