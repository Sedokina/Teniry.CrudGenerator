using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.EntityCustomization;

namespace ITech.CrudGenerator.Tests.BuilderFactories;

public class GetListQueryDefaultConfigurationBuilderFactoryTests
{
    private readonly GetListQueryDefaultConfigurationBulderFactory _sut;
    private readonly GlobalCqrsGeneratorConfigurationBuilder _globalCqrsGeneratorConfigurationBuilder;
    private readonly CqrsOperationsSharedConfigurationBuilder _cqrsOperationsSharedConfigurationBuilder;

    public GetListQueryDefaultConfigurationBuilderFactoryTests()
    {
        _sut = new GetListQueryDefaultConfigurationBulderFactory();
        _globalCqrsGeneratorConfigurationBuilder = new GlobalCqrsGeneratorConfigurationBuilder();
        _cqrsOperationsSharedConfigurationBuilder = new CqrsOperationsSharedConfigurationBuilder();
    }

    [Fact]
    public void Should_PutGlobalAndSharedConfigurationsIntoBuiltConfiguration()
    {
        // Act
        var actual = _sut.Construct(
            _globalCqrsGeneratorConfigurationBuilder,
            _cqrsOperationsSharedConfigurationBuilder,
            new EntityGetListOperationCustomizationScheme());

        // Assert
        actual.GlobalConfiguration.Should().Be(_globalCqrsGeneratorConfigurationBuilder);
        actual.OperationsSharedConfiguration.Should().Be(_cqrsOperationsSharedConfigurationBuilder);
    }

    [Fact]
    public void Should_SetCorrectDefaultValues()
    {
        // Arrange
        var entityName = new EntityName("TestEntity", "TestEntities");
        var operationName = "Get";
        var path = "AllFiles";

        // Act
        var actual = _sut.Construct(
            _globalCqrsGeneratorConfigurationBuilder,
            _cqrsOperationsSharedConfigurationBuilder,
            new EntityGetListOperationCustomizationScheme());

        // Assert
        actual.Generate.Should().BeTrue();
        actual.OperationType.Should().Be(CqrsOperationType.Query);
        actual.OperationName.Should().Be(operationName);
        actual.OperationGroup.GetName(entityName, operationName).Should().Be("GetTestEntities");
        actual.Operation.TemplatePath.GetPath(path, "").Should().Be("AllFiles.GetList.GetListQuery.txt");
        actual.Operation.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("GetTestEntitiesQuery");
        actual.Dto.TemplatePath.GetPath(path, "").Should().Be("AllFiles.GetList.GetListDto.txt");
        actual.Dto.NameConfigurationBuilder.GetName(entityName, operationName).Should().Be("TestEntitiesDto");
        actual.Handler.TemplatePath.GetPath(path, "").Should().Be("AllFiles.GetList.GetListHandler.txt");
        actual.Handler.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("GetTestEntitiesHandler");
        actual.Endpoint.TemplatePath.GetPath(path, "").Should().Be("AllFiles.GetList.GetListEndpoint.txt");
        actual.Endpoint.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("GetTestEntitiesEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.GetName(entityName, operationName).Should().Be("GetAsync");
        actual.Endpoint.RouteConfigurationBuilder.GetRoute(entityName.Name, operationName, [])
            .Should().Be("/testentity");
    }

    [Fact]
    public void Should_CustomizeAllConfigurationWithOperationName_When_OperationNameSetInCustomizationScheme()
    {
        // Arrange
        var entityName = new EntityName("TestEntity", "TestEntities");
        var operationName = "Obtain";
        var path = "AllFiles";
        var entityGetListOperationCustomizationScheme = new EntityGetListOperationCustomizationScheme
        {
            Operation = operationName
        };

        // Act
        var actual = _sut.Construct(
            _globalCqrsGeneratorConfigurationBuilder,
            _cqrsOperationsSharedConfigurationBuilder,
            entityGetListOperationCustomizationScheme);

        // Assert
        actual.Generate.Should().BeTrue();
        actual.OperationType.Should().Be(CqrsOperationType.Query);
        actual.OperationName.Should().Be(operationName);
        actual.OperationGroup.GetName(entityName, operationName).Should().Be("ObtainTestEntities");
        actual.Operation.TemplatePath.GetPath(path, "").Should().Be("AllFiles.GetList.GetListQuery.txt");
        actual.Operation.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("ObtainTestEntitiesQuery");
        actual.Dto.TemplatePath.GetPath(path, "").Should().Be("AllFiles.GetList.GetListDto.txt");
        actual.Dto.NameConfigurationBuilder.GetName(entityName, operationName).Should().Be("TestEntitiesDto");
        actual.Handler.TemplatePath.GetPath(path, "").Should().Be("AllFiles.GetList.GetListHandler.txt");
        actual.Handler.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("ObtainTestEntitiesHandler");
        actual.Endpoint.TemplatePath.GetPath(path, "").Should().Be("AllFiles.GetList.GetListEndpoint.txt");
        actual.Endpoint.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("ObtainTestEntitiesEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.GetName(entityName, operationName).Should().Be("ObtainAsync");
        actual.Endpoint.RouteConfigurationBuilder.GetRoute(entityName.Name, operationName, [])
            .Should().Be("/testentity");
    }
    
     [Fact]
    public void Should_CustomizeAllAvailableConfiguration()
    {
        // Arrange
        var entityName = new EntityName("TestEntity", "TestEntities");
        var operationName = "Get";
        var path = "AllFiles";
        var entityGetListOperationCustomizationScheme = new EntityGetListOperationCustomizationScheme
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
        var actual = _sut.Construct(
            _globalCqrsGeneratorConfigurationBuilder,
            _cqrsOperationsSharedConfigurationBuilder,
            entityGetListOperationCustomizationScheme);

        // Assert
        actual.Generate.Should().BeFalse();
        actual.OperationType.Should().Be(CqrsOperationType.Query);
        actual.OperationName.Should().Be(operationName);
        actual.OperationGroup.GetName(entityName, operationName).Should().Be("CustomOperationGroupName");
        actual.Operation.TemplatePath.GetPath(path, "").Should().Be("AllFiles.GetList.GetListQuery.txt");
        actual.Operation.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("CustomQueryName");
        actual.Dto.TemplatePath.GetPath(path, "").Should().Be("AllFiles.GetList.GetListDto.txt");
        actual.Dto.NameConfigurationBuilder.GetName(entityName, operationName).Should().Be("CustomDtoName");
        actual.Handler.TemplatePath.GetPath(path, "").Should().Be("AllFiles.GetList.GetListHandler.txt");
        actual.Handler.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("CustomHandlerName");
        actual.Endpoint.TemplatePath.GetPath(path, "").Should().Be("AllFiles.GetList.GetListEndpoint.txt");
        actual.Endpoint.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("CustomEndpointClassName");
        actual.Endpoint.Generate.Should().BeFalse();
        actual.Endpoint.FunctionName.GetName(entityName, operationName).Should().Be("CustomEndpointFunctionName");
        actual.Endpoint.RouteConfigurationBuilder.GetRoute(entityName.Name, operationName, [])
            .Should().Be("CustomEndpointRoute");
    }
}