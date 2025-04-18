using Teniry.CrudGenerator.Core.Configurations.Crud.TypedConfigurations;
using Teniry.CrudGenerator.Core.Configurations.Global;
using Teniry.CrudGenerator.Core.Configurations.Shared;
using Teniry.CrudGenerator.Core.Runners;
using Teniry.CrudGenerator.Core.Schemes.Entity;
using Teniry.CrudGenerator.Core.Schemes.InternalEntityGenerator;
using Teniry.CrudGenerator.Core.Schemes.InternalEntityGenerator.Operations;
using Teniry.CrudGenerator.Tests.Helpers;

namespace Teniry.CrudGenerator.Tests.GeneratorRunners;

public class GetListQueryGeneratorRunnerTests {
    private readonly EntityScheme _entityScheme;

    public GetListQueryGeneratorRunnerTests() {
        var internalEntityGeneratorConfiguration =
            new InternalEntityGeneratorConfiguration(new("TestEntity", "", "", []));
        _entityScheme = EntitySchemeFactory.Construct(internalEntityGeneratorConfiguration, new DbContextSchemeStub());
    }

    [Fact]
    public void Should_PutGlobalAndSharedConfigurationsIntoBuiltConfiguration() {
        // Arrange
        var sut = CreateFactory(new());

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
    public void Should_SetCorrectDefaultValues() {
        // Arrange
        var sut = CreateFactory(new());

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
    public void Should_CustomizeAllConfigurationWithOperationName_When_OperationNameSetInGeneratorConfiguration() {
        // Arrange
        var sut = CreateFactory(
            new() {
                Operation = "Obtain"
            }
        );

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
    public void Should_CustomizeAllAvailableConfiguration() {
        // Arrange
        var sut = CreateFactory(
            new() {
                Generate = false,
                OperationGroup = "CustomOperationGroupName",
                QueryName = "CustomQueryName",
                DtoName = "CustomDtoName",
                HandlerName = "CustomHandlerName",
                EndpointClassName = "CustomEndpointClassName",
                EndpointFunctionName = "CustomEndpointFunctionName",
                GenerateEndpoint = false,
                RouteName = "CustomEndpointRoute"
            }
        );

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
        InternalEntityGeneratorGetListOperationConfiguration configuration
    ) {
        return new(
            GlobalCrudGeneratorConfigurationFactory.Construct(),
            new CqrsOperationsSharedConfiguratorFactory().Construct(),
            configuration,
            _entityScheme,
            new DbContextSchemeStub()
        );
    }
}