using ITech.CrudGenerator.Core.Configurations.Crud.TypedConfigurations;
using ITech.CrudGenerator.Core.Configurations.Global;
using ITech.CrudGenerator.Core.Configurations.Shared;
using ITech.CrudGenerator.Core.Runners;
using ITech.CrudGenerator.Core.Schemes.Entity;
using ITech.CrudGenerator.Core.Schemes.InternalEntityGenerator;
using ITech.CrudGenerator.Core.Schemes.InternalEntityGenerator.Operations;
using ITech.CrudGenerator.Tests.Helpers;
using Microsoft.CodeAnalysis;

namespace ITech.CrudGenerator.Tests.GeneratorRunners;

public class DeleteCommandGeneratorRunnerTests {
    private readonly EntityScheme _entityScheme;

    public DeleteCommandGeneratorRunnerTests() {
        var internalEntityGeneratorConfiguration = new InternalEntityGeneratorConfiguration(
            new InternalEntityClassMetadata(
                "TestEntity",
                "",
                "",
                [
                    new InternalEntityClassPropertyMetadata("Id", "Guid", "Guid", SpecialType.None, true, false)
                ]
            )
        );
        _entityScheme = EntitySchemeFactory.Construct(internalEntityGeneratorConfiguration, new DbContextSchemeStub());
    }

    [Fact]
    public void Should_PutGlobalAndSharedConfigurationsIntoBuiltConfiguration() {
        // Arrange
        var sut = CreateFactory(new InternalEntityGeneratorDeleteOperationConfiguration());

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
        var sut = CreateFactory(new InternalEntityGeneratorDeleteOperationConfiguration());

        // Act
        var actual = sut.Configuration;

        // Assert
        actual.Generate.Should().BeTrue();
        actual.OperationType.Should().Be(CqrsOperationType.Command);
        actual.OperationName.Should().Be("Delete");
        actual.OperationGroup.Should().Be("DeleteTestEntity");
        actual.Operation.Should().Be("DeleteTestEntityCommand");
        actual.Handler.Should().Be("DeleteTestEntityHandler");
        actual.Endpoint.Name.Should().Be("DeleteTestEntityEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.Should().Be("DeleteAsync");
        actual.Endpoint.Route.Should().Be("/testEntity/{id}/delete");
    }

    [Fact]
    public void Should_CustomizeAllConfigurationWithOperationName_When_OperationNameSetInGeneratorConfiguration() {
        // Arrange
        var sut = CreateFactory(
            new InternalEntityGeneratorDeleteOperationConfiguration {
                Operation = "Del"
            }
        );

        // Act
        var actual = sut.Configuration;

        // Assert
        actual.Generate.Should().BeTrue();
        actual.OperationType.Should().Be(CqrsOperationType.Command);
        actual.OperationName.Should().Be("Del");
        actual.OperationGroup.Should().Be("DelTestEntity");
        actual.Operation.Should().Be("DelTestEntityCommand");
        actual.Handler.Should().Be("DelTestEntityHandler");
        actual.Endpoint.Name.Should().Be("DelTestEntityEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.Should().Be("DelAsync");
        actual.Endpoint.Route.Should().Be("/testEntity/{id}/del");
    }

    [Fact]
    public void Should_CustomizeAllAvailableConfiguration() {
        // Arrange
        var sut = CreateFactory(
            new InternalEntityGeneratorDeleteOperationConfiguration {
                Generate = false,
                OperationGroup = "CustomOperationGroupName",
                CommandName = "CustomCommandName",
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
        actual.OperationType.Should().Be(CqrsOperationType.Command);
        actual.OperationName.Should().Be("Delete");
        actual.OperationGroup.Should().Be("CustomOperationGroupName");
        actual.Operation.Should().Be("CustomCommandName");
        actual.Handler.Should().Be("CustomHandlerName");
        actual.Endpoint.Name.Should().Be("CustomEndpointClassName");
        actual.Endpoint.Generate.Should().BeFalse();
        actual.Endpoint.FunctionName.Should().Be("CustomEndpointFunctionName");
        actual.Endpoint.Route.Should().Be("CustomEndpointRoute");
    }

    private DeleteCommandGeneratorRunner CreateFactory(
        InternalEntityGeneratorDeleteOperationConfiguration configuration
    ) {
        return new DeleteCommandGeneratorRunner(
            GlobalCrudGeneratorConfigurationFactory.Construct(),
            new CqrsOperationsSharedConfiguratorFactory().Construct(),
            configuration,
            _entityScheme,
            new DbContextSchemeStub()
        );
    }
}