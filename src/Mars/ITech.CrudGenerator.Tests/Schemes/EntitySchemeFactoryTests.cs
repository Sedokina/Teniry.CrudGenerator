using System.Reflection;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.DbContext;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Expressions;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator;
using ITech.CrudGenerator.Tests.Helpers;
using Microsoft.CodeAnalysis;

namespace ITech.CrudGenerator.Tests.Schemes;

public class EntitySchemeFactoryTests
{
    private readonly EntitySchemeFactory _sut;
    private readonly InternalEntityGeneratorConfiguration _internalEntityGeneratorConfiguration;
    private readonly DbContextScheme _dbContextScheme;

    public EntitySchemeFactoryTests()
    {
        _sut = new();
        _internalEntityGeneratorConfiguration = new(new InternalEntityClassMetadata("MyEntityName", "", "", []));
        _dbContextScheme = new DbContextSchemeStub();
    }

    [Fact]
    public void Should_GetEntityNameFromSymbol()
    {
        // Arrange
        _internalEntityGeneratorConfiguration.ClassMetadata.ClassName = "MyEntityName";

        // Act
        var actual = _sut.Construct(_internalEntityGeneratorConfiguration, _dbContextScheme);

        // Assert
        actual.EntityName.Name.Should().Be("MyEntityName");
    }

    [Theory]
    [InlineData("Currency", "Currencies")]
    [InlineData("Job", "Jobs")]
    [InlineData("Box", "Boxes")]
    [InlineData("DepartmentGroup", "DepartmentGroups")]
    public void Should_GeneratePluralName_From_EntityName(string singular, string plural)
    {
        // Arrange
        _internalEntityGeneratorConfiguration.ClassMetadata.ClassName = singular;

        // Act
        var actual = _sut.Construct(_internalEntityGeneratorConfiguration, _dbContextScheme);

        // Assert
        actual.EntityName.PluralName.Should().Be(plural);
    }

    [Theory]
    [InlineData("Series", "SeriesList")]
    [InlineData("Sheep", "SheepList")]
    [InlineData("Staff", "StaffList")]
    [InlineData("Employees", "EmployeesList")]
    [InlineData("Currencies", "CurrenciesList")]
    [InlineData("DepartmentGroups", "DepartmentGroupsList")]
    public void Should_AddSuffixToEntityPluralName_When_PluralAndSingularFormAreSame(string singular, string plural)
    {
        // Arrange
        _internalEntityGeneratorConfiguration.ClassMetadata.ClassName = singular;

        // Act
        var actual = _sut.Construct(_internalEntityGeneratorConfiguration, _dbContextScheme);

        // Assert
        actual.EntityName.PluralName.Should().Be(plural);
    }

    [Fact]
    public void Should_GenerateTitle_From_EntityName()
    {
        // Arrange
        _internalEntityGeneratorConfiguration.ClassMetadata.ClassName = "MyEntityName";

        // Act
        var actual = _sut.Construct(_internalEntityGeneratorConfiguration, _dbContextScheme);

        // Assert
        actual.EntityTitle.Title.Should().Be("My entity name");
    }

    [Theory]
    [InlineData("Currency", "Currencies")]
    [InlineData("DepartmentGroup", "Department groups")]
    [InlineData("Currencies", "Currencies list")]
    [InlineData("BooksGroups", "Books groups list")]
    public void Should_GeneratePluralTitle_From_EntityPluralName(string entityName, string pluralTitle)
    {
        // Arrange
        _internalEntityGeneratorConfiguration.ClassMetadata.ClassName = entityName;

        // Act
        var actual = _sut.Construct(_internalEntityGeneratorConfiguration, _dbContextScheme);

        // Assert
        actual.EntityTitle.PluralTitle.Should().Be(pluralTitle);
    }

    [Fact]
    public void Should_ExtractEntityNamespaceAndAssembly()
    {
        // Arrange
        _internalEntityGeneratorConfiguration.ClassMetadata.ContainingNamespace = "ITech.CrudGenerator.Tests";
        _internalEntityGeneratorConfiguration.ClassMetadata.ContainingAssembly =
            Assembly.GetExecutingAssembly().FullName ?? "";

        // Act
        var actual = _sut.Construct(_internalEntityGeneratorConfiguration, _dbContextScheme);

        // Assert
        actual.EntityNamespace.Should().Be("ITech.CrudGenerator.Tests");
        actual.ContainingAssembly.Should().Be(Assembly.GetExecutingAssembly().FullName);
    }

    [Fact]
    public void Should_NotHaveDefaultSort_When_DefaultSortNotSetInInternalEntityGeneratorConfiguration()
    {
        // Arrange
        _internalEntityGeneratorConfiguration.DefaultSort = null;

        // Act
        var actual = _sut.Construct(_internalEntityGeneratorConfiguration, _dbContextScheme);

        // Assert
        actual.DefaultSort.Should().BeNull();
    }

    [Fact]
    public void Should_UseDefaultSort_From_InternalEntityGeneratorConfiguration()
    {
        // Arrange
        _internalEntityGeneratorConfiguration.DefaultSort = new EntityDefaultSort("asc", "MyProp");

        // Act
        var actual = _sut.Construct(_internalEntityGeneratorConfiguration, _dbContextScheme);

        // Assert
        actual.DefaultSort.Should().NotBeNull();
        actual.DefaultSort!.PropertyName.Should().Be("MyProp");
        actual.DefaultSort.Direction.Should().Be("asc");
    }

    [Fact]
    public void Should_IgnoreReferenceProperties()
    {
        // Arrange
        _internalEntityGeneratorConfiguration.ClassMetadata.Properties =
        [
            new("ReferenceProp", "EntitySchemeFactoryTests", "", SpecialType.System_TypedReference, false, false),
        ];

        // Act
        var actual = _sut.Construct(_internalEntityGeneratorConfiguration, _dbContextScheme);

        // Assert
        actual.Properties.Should().NotContain(x => x.PropertyName == "ReferenceProp");
    }

    [Fact]
    public void Should_ExtractSimpleProperty()
    {
        // Arrange
        _internalEntityGeneratorConfiguration.ClassMetadata.Properties =
        [
            new("SimpleProp", "int", "", SpecialType.System_Int32, true, false),
        ];

        // Act
        var actual = _sut.Construct(_internalEntityGeneratorConfiguration, _dbContextScheme);

        // Assert
        actual.Properties.Should().Contain(x => x.PropertyName == "SimpleProp");
    }

    [Fact]
    public void Should_IgnoreSystemPrefixInPropertyTypes()
    {
        // Arrange
        _internalEntityGeneratorConfiguration.ClassMetadata.Properties =
        [
            new("DateTimeProp", "System.DateTimeOffset", "DateTimeOffset", SpecialType.System_DateTime, true, false),
        ];

        // Act
        var actual = _sut.Construct(_internalEntityGeneratorConfiguration, _dbContextScheme);

        // Assert
        actual.Properties.Should()
            .Contain(x => x.PropertyName == "DateTimeProp" && x.TypeName.Equals("DateTimeOffset"));
    }

    [Fact]
    public void Should_HaveDefaultValueForString()
    {
        // Arrange
        _internalEntityGeneratorConfiguration.ClassMetadata.Properties =
        [
            new("StringProp", "string", "", SpecialType.System_String, true, false),
        ];

        // Act
        var actual = _sut.Construct(_internalEntityGeneratorConfiguration, _dbContextScheme);

        // Assert
        actual.Properties.Should().Contain(x => x.PropertyName == "StringProp" &&
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
        _internalEntityGeneratorConfiguration.ClassMetadata.Properties =
        [
            new(propertyName, "Guid", "Guid", SpecialType.None, true, false),
        ];

        // Act
        var actual = _sut.Construct(_internalEntityGeneratorConfiguration, _dbContextScheme);

        // Assert
        actual.Properties.Should().Contain(x => x.PropertyName == propertyName &&
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
        _internalEntityGeneratorConfiguration.ClassMetadata.Properties =
        [
            new("SortableProperty", propertyType, propertyType, SpecialType.None, true, false),
        ];

        // Act
        var actual = _sut.Construct(_internalEntityGeneratorConfiguration, _dbContextScheme);

        // Assert
        actual.Properties.Should().Contain(x => x.PropertyName == "SortableProperty" && x.CanBeSorted == true);
    }

    [Fact]
    public void Should_NameSortablePropertyKeyInCamelCase()
    {
        // Arrange
        _internalEntityGeneratorConfiguration.ClassMetadata.Properties =
        [
            new("SortableProperty", "int", "int", SpecialType.System_Int32, true, false),
        ];

        // Act
        var actual = _sut.Construct(_internalEntityGeneratorConfiguration, _dbContextScheme);

        // Assert
        actual.Properties.Should()
            .Contain(x => x.PropertyName == "SortableProperty" && x.SortKey.Equals("sortableProperty"));
    }

    [Theory]
    [InlineData("int", "MyEntityNameId", "MyEntityNameIds", SpecialType.System_Int32)]
    [InlineData("int", "Id", "Ids", SpecialType.System_Int32)]
    [InlineData("int", "_id", "_ids", SpecialType.System_Int32)]
    [InlineData("int", "OtherEntityId", "OtherEntityIds", SpecialType.System_Int32)]
    [InlineData("int", "OtherEntity_id", "OtherEntity_ids", SpecialType.System_Int32)]
    [InlineData("Guid", "MyEntityNameId", "MyEntityNameIds", SpecialType.None)]
    [InlineData("Guid", "Id", "Ids", SpecialType.None)]
    [InlineData("Guid", "_id", "_ids", SpecialType.None)]
    [InlineData("Guid", "OtherEntityId", "OtherEntityIds", SpecialType.None)]
    [InlineData("Guid", "OtherEntity_id", "OtherEntity_ids", SpecialType.None)]
    public void Should_HaveContainsFilter_ForPrimaryOrForeignKey(
        string typeName,
        string propertyName,
        string expectedFilterPropertyName,
        SpecialType specialType)
    {
        // Arrange
        _internalEntityGeneratorConfiguration.ClassMetadata.Properties =
        [
            new(propertyName, typeName, typeName, specialType, true, false),
        ];

        // Act
        var actual = _sut.Construct(_internalEntityGeneratorConfiguration, _dbContextScheme);

        // Assert
        actual.Properties.Should()
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
    [InlineData("sbyte", SpecialType.System_SByte)]
    [InlineData("byte", SpecialType.System_Byte)]
    [InlineData("short", SpecialType.System_Int16)]
    [InlineData("int", SpecialType.System_Int32)]
    [InlineData("long", SpecialType.System_Int64)]
    [InlineData("ushort", SpecialType.System_UInt16)]
    [InlineData("uint", SpecialType.System_UInt32)]
    [InlineData("ulong", SpecialType.System_UInt64)]
    [InlineData("float", SpecialType.System_Single)]
    [InlineData("double", SpecialType.System_Double)]
    [InlineData("decimal", SpecialType.System_Decimal)]
    [InlineData("DateTime", SpecialType.System_DateTime)]
    [InlineData("DateTimeOffset", SpecialType.None)]
    public void Should_HaveRangeFilter_ForSimpleTypes(string typeName, SpecialType specialType)
    {
        // Arrange
        var propertyName = "FilteredProperty";
        _internalEntityGeneratorConfiguration.ClassMetadata.Properties =
        [
            new(propertyName, typeName, typeName, specialType, true, false),
        ];

        // Act
        var actual = _sut.Construct(_internalEntityGeneratorConfiguration, _dbContextScheme);

        // Assert
        actual.Properties.Should()
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

    [Theory]
    [InlineData("string", SpecialType.System_String, typeof(LikeFilterExpression))]
    [InlineData("char", SpecialType.System_Char, typeof(EqualsFilterExpression))]
    public void Should_HaveLikeFilter_ForStringType(string typeName, SpecialType specialType, Type filterExpressionType)
    {
        // Arrange
        var propertyName = "FilteredProperty";
        _internalEntityGeneratorConfiguration.ClassMetadata.Properties =
        [
            new(propertyName, typeName, typeName, specialType, true, false),
        ];

        // Act
        var actual = _sut.Construct(_internalEntityGeneratorConfiguration, _dbContextScheme);

        // Assert
        actual.Properties.Should()
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
                        f.FilterExpression.Should().BeOfType(filterExpressionType);
                    });
            });
    }
}