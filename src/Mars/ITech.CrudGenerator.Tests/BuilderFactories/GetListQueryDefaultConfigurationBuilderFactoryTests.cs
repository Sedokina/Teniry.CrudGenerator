using ITech.CrudGenerator.Abstractions.DbContext;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.DbContext;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Core;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator.Operations;
using ITech.CrudGenerator.Tests.Helpers;

namespace ITech.CrudGenerator.Tests.BuilderFactories;

public class GetListQueryDefaultFactoryTests
{
    private readonly GetListQueryDefaultConfigurationBulderFactory _sut;
    private readonly GlobalCqrsGeneratorConfigurationBuilder _globalCqrsGeneratorConfigurationBuilder;
    private readonly CqrsOperationsSharedConfigurationBuilder _cqrsOperationsSharedConfigurationBuilder;
    private readonly EntityScheme _entityScheme;

    public GetListQueryDefaultFactoryTests()
    {
        _sut = new GetListQueryDefaultConfigurationBulderFactory();
        _globalCqrsGeneratorConfigurationBuilder = new GlobalCqrsGeneratorConfigurationBuilder
            { TemplatesBasePath = "AllFiles" };
        _cqrsOperationsSharedConfigurationBuilder = new CqrsOperationsSharedConfigurationBuilderFactory().Construct();
        var entitySchemeFactory = new EntitySchemeFactory();
        var symbol = DynamicClassBuilder.GenerateEntity("TestEntity", "public Guid Id {{ get; set; }}");
        _entityScheme = entitySchemeFactory.Construct(symbol, new InternalEntityGeneratorConfiguration(),
            new DbContextScheme("", "", DbContextDbProvider.Mongo, new Dictionary<FilterType, FilterExpression>()));
    }

    [Fact]
    public void Should_PutGlobalAndSharedConfigurationsIntoBuiltConfiguration()
    {
        // Act
        var actual = _sut.Construct(
            _globalCqrsGeneratorConfigurationBuilder,
            _cqrsOperationsSharedConfigurationBuilder,
            new InternalEntityGeneratorGetListOperationConfiguration());

        // Assert
        actual.GlobalConfiguration.Should().Be(_globalCqrsGeneratorConfigurationBuilder);
        actual.OperationsSharedConfiguration.Should().Be(_cqrsOperationsSharedConfigurationBuilder);
    }

    [Fact]
    public void Should_SetCorrectDefaultValues()
    {
        // Act
        var actual = _sut
            .Construct(
                _globalCqrsGeneratorConfigurationBuilder,
                _cqrsOperationsSharedConfigurationBuilder,
                new InternalEntityGeneratorGetListOperationConfiguration())
            .Build(_entityScheme);

        // Assert
        actual.Generate.Should().BeTrue();
        actual.OperationType.Should().Be(CqrsOperationType.Query);
        actual.OperationName.Should().Be("Get");
        actual.OperationGroup.Should().Be("GetTestEntities");
        actual.Operation.TemplatePath.Should().Be("AllFiles.GetList.GetListQuery.txt");
        actual.Operation.Name.Should().Be("GetTestEntitiesQuery");
        actual.Dto.TemplatePath.Should().Be("AllFiles.GetList.GetListDto.txt");
        actual.Dto.Name.Should().Be("TestEntitiesDto");
        actual.Handler.TemplatePath.Should().Be("AllFiles.GetList.GetListHandler.txt");
        actual.Handler.Name.Should().Be("GetTestEntitiesHandler");
        actual.Endpoint.TemplatePath.Should().Be("AllFiles.GetList.GetListEndpoint.txt");
        actual.Endpoint.Name.Should().Be("GetTestEntitiesEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.Should().Be("GetAsync");
        actual.Endpoint.Route.Should().Be("/testentity");
    }

    [Fact]
    public void Should_CustomizeAllConfigurationWithOperationName_When_OperationNameSetInGeneratorConfiguration()
    {
        // Arrange
        var operationConfiguration = new InternalEntityGeneratorGetListOperationConfiguration
        {
            Operation = "Obtain"
        };

        // Act
        var actual = _sut
            .Construct(
                _globalCqrsGeneratorConfigurationBuilder,
                _cqrsOperationsSharedConfigurationBuilder,
                operationConfiguration)
            .Build(_entityScheme);

        // Assert
        actual.Generate.Should().BeTrue();
        actual.OperationType.Should().Be(CqrsOperationType.Query);
        actual.OperationName.Should().Be("Obtain");
        actual.OperationGroup.Should().Be("ObtainTestEntities");
        actual.Operation.TemplatePath.Should().Be("AllFiles.GetList.GetListQuery.txt");
        actual.Operation.Name.Should().Be("ObtainTestEntitiesQuery");
        actual.Dto.TemplatePath.Should().Be("AllFiles.GetList.GetListDto.txt");
        actual.Dto.Name.Should().Be("TestEntitiesDto");
        actual.Handler.TemplatePath.Should().Be("AllFiles.GetList.GetListHandler.txt");
        actual.Handler.Name.Should().Be("ObtainTestEntitiesHandler");
        actual.Endpoint.TemplatePath.Should().Be("AllFiles.GetList.GetListEndpoint.txt");
        actual.Endpoint.Name.Should().Be("ObtainTestEntitiesEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.Should().Be("ObtainAsync");
        actual.Endpoint.Route.Should().Be("/testentity");
    }

    [Fact]
    public void Should_CustomizeAllAvailableConfiguration()
    {
        // Arrange
        var operationConfiguration = new InternalEntityGeneratorGetListOperationConfiguration
        {
            Generate = false,
            OperationGroup = "CustomOperationGroupName",
            QueryName = "CustomQueryName",
            DtoName = "CustomDtoName",
            HandlerName = "CustomHandlerName",
            EndpointClassName = "CustomEndpointClassName",
            EndpointFunctionName = "CustomEndpointFunctionName",
            GenerateEndpoint = false,
            RouteName = "CustomEndpointRoute"
        };

        // Act
        var actual = _sut
            .Construct(
                _globalCqrsGeneratorConfigurationBuilder,
                _cqrsOperationsSharedConfigurationBuilder,
                operationConfiguration)
            .Build(_entityScheme);

        // Assert
        actual.Generate.Should().BeFalse();
        actual.OperationType.Should().Be(CqrsOperationType.Query);
        actual.OperationName.Should().Be("Get");
        actual.OperationGroup.Should().Be("CustomOperationGroupName");
        actual.Operation.TemplatePath.Should().Be("AllFiles.GetList.GetListQuery.txt");
        actual.Operation.Name.Should().Be("CustomQueryName");
        actual.Dto.TemplatePath.Should().Be("AllFiles.GetList.GetListDto.txt");
        actual.Dto.Name.Should().Be("CustomDtoName");
        actual.Handler.TemplatePath.Should().Be("AllFiles.GetList.GetListHandler.txt");
        actual.Handler.Name.Should().Be("CustomHandlerName");
        actual.Endpoint.TemplatePath.Should().Be("AllFiles.GetList.GetListEndpoint.txt");
        actual.Endpoint.Name.Should().Be("CustomEndpointClassName");
        actual.Endpoint.Generate.Should().BeFalse();
        actual.Endpoint.FunctionName.Should().Be("CustomEndpointFunctionName");
        actual.Endpoint.Route.Should().Be("CustomEndpointRoute");
    }
}