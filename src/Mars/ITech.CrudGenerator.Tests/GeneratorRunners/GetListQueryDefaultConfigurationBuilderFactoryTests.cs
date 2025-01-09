using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Crud.TypedConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Shared;
using ITech.CrudGenerator.CrudGeneratorCore.GeneratorRunners;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator.Operations;
using ITech.CrudGenerator.Tests.Helpers;

namespace ITech.CrudGenerator.Tests.GeneratorRunners;

public class GetListQueryGeneratorRunnerTests
{
    private readonly EntityScheme _entityScheme;

    public GetListQueryGeneratorRunnerTests()
    {
        var internalEntityGeneratorConfiguration =
            new InternalEntityGeneratorConfiguration(new InternalEntityClassMetadata("TestEntity", "", "", []));
        _entityScheme = EntitySchemeFactory.Construct(internalEntityGeneratorConfiguration, new DbContextSchemeStub());
    }

    [Fact]
    public void Should_PutGlobalAndSharedConfigurationsIntoBuiltConfiguration()
    {
        // Arrange
        var sut = CreateFactory(new InternalEntityGeneratorGetListOperationConfiguration());

        // Act
        var actual = sut.Configuration;

        // Assert
        actual.GlobalConfiguration.NullableEnable.Should().BeTrue();
        actual.GlobalConfiguration.AutogeneratedFileText.Should().NotBeEmpty();
        actual.OperationsSharedConfiguration.BusinessLogicFeatureName.Should().NotBeEmpty();
        actual.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation.Should().NotBeEmpty();
        actual.OperationsSharedConfiguration.EndpointsNamespaceForFeature.Should().NotBeEmpty();
    }

    [Fact]
    public void Should_SetCorrectDefaultValues()
    {
        // Arrange
        var sut = CreateFactory(new InternalEntityGeneratorGetListOperationConfiguration());

        // Act
        var actual = sut.Configuration;

        // Assert
        actual.Generate.Should().BeTrue();
        actual.OperationType.Should().Be(CqrsOperationType.Query);
        actual.OperationName.Should().Be("Get");
        actual.OperationGroup.Should().Be("GetTestEntities");
        actual.Operation.Should().Be("GetTestEntitiesQuery");
        actual.Dto.Should().Be("TestEntitiesDto");
        actual.Handler.Should().Be("GetTestEntitiesHandler");
        actual.Endpoint.Name.Should().Be("GetTestEntitiesEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.Should().Be("GetAsync");
        actual.Endpoint.Route.Should().Be("/testEntity");
    }

    [Fact]
    public void Should_CustomizeAllConfigurationWithOperationName_When_OperationNameSetInGeneratorConfiguration()
    {
        // Arrange
        var sut = CreateFactory(new InternalEntityGeneratorGetListOperationConfiguration
        {
            Operation = "Obtain"
        });

        // Act
        var actual = sut.Configuration;

        // Assert
        actual.Generate.Should().BeTrue();
        actual.OperationType.Should().Be(CqrsOperationType.Query);
        actual.OperationName.Should().Be("Obtain");
        actual.OperationGroup.Should().Be("ObtainTestEntities");
        actual.Operation.Should().Be("ObtainTestEntitiesQuery");
        actual.Dto.Should().Be("TestEntitiesDto");
        actual.Handler.Should().Be("ObtainTestEntitiesHandler");
        actual.Endpoint.Name.Should().Be("ObtainTestEntitiesEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.Should().Be("ObtainAsync");
        actual.Endpoint.Route.Should().Be("/testEntity");
    }

    [Fact]
    public void Should_CustomizeAllAvailableConfiguration()
    {
        // Arrange
        var sut = CreateFactory(new InternalEntityGeneratorGetListOperationConfiguration
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
        });

        // Act
        var actual = sut.Configuration;

        // Assert
        actual.Generate.Should().BeFalse();
        actual.OperationType.Should().Be(CqrsOperationType.Query);
        actual.OperationName.Should().Be("Get");
        actual.OperationGroup.Should().Be("CustomOperationGroupName");
        actual.Operation.Should().Be("CustomQueryName");
        actual.Dto.Should().Be("CustomDtoName");
        actual.Handler.Should().Be("CustomHandlerName");
        actual.Endpoint.Name.Should().Be("CustomEndpointClassName");
        actual.Endpoint.Generate.Should().BeFalse();
        actual.Endpoint.FunctionName.Should().Be("CustomEndpointFunctionName");
        actual.Endpoint.Route.Should().Be("CustomEndpointRoute");
    }

    private GetListQueryGeneratorRunner CreateFactory(
        InternalEntityGeneratorGetListOperationConfiguration configuration)
    {
        return new GetListQueryGeneratorRunner(
            GlobalCrudGeneratorConfigurationFactory.Construct(),
            new CqrsOperationsSharedConfigurationBuilderFactory().Construct(),
            configuration,
            _entityScheme,
            new DbContextSchemeStub()
        );
    }
}