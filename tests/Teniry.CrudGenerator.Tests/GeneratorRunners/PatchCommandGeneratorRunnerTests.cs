using Teniry.CrudGenerator.Core.Configurations.Crud.TypedConfigurations;
using Teniry.CrudGenerator.Core.Configurations.Global;
using Teniry.CrudGenerator.Core.Configurations.Shared;
using Teniry.CrudGenerator.Core.Runners;
using Teniry.CrudGenerator.Core.Schemes.Entity;
using Teniry.CrudGenerator.Core.Schemes.InternalEntityGenerator;
using Teniry.CrudGenerator.Core.Schemes.InternalEntityGenerator.Operations;
using Microsoft.CodeAnalysis;
using Teniry.CrudGenerator.Tests.Helpers;

namespace Teniry.CrudGenerator.Tests.GeneratorRunners;

public class PatchCommandGeneratorRunnerTests {
    private readonly EntityScheme _entityScheme;

    public PatchCommandGeneratorRunnerTests() {
        var internalEntityGeneratorConfiguration = new InternalEntityGeneratorConfiguration(
            new(
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
        actual.OperationType.Should().Be(CqrsOperationType.Command);
        actual.OperationName.Should().Be("Patch");
        actual.OperationGroup.Should().Be("PatchTestEntity");
        actual.Operation.Should().Be("PatchTestEntityCommand");
        actual.Handler.Should().Be("PatchTestEntityHandler");
        actual.ViewModel.Should().Be("PatchTestEntityVm");
        actual.Endpoint.Name.Should().Be("PatchTestEntityEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.Should().Be("PatchAsync");
        actual.Endpoint.Route.Should().Be("/testEntity/{id}/patch");
    }

    [Fact]
    public void Should_CustomizeAllConfigurationWithOperationName_When_OperationNameSetInGeneratorConfiguration() {
        // Arrange
        var sut = CreateFactory(
            new() {
                Operation = "Upd"
            }
        );

        // Act
        var actual = sut.Configuration;

        // Assert
        actual.Generate.Should().BeTrue();
        actual.OperationType.Should().Be(CqrsOperationType.Command);
        actual.OperationName.Should().Be("Upd");
        actual.OperationGroup.Should().Be("UpdTestEntity");
        actual.Operation.Should().Be("UpdTestEntityCommand");
        actual.Handler.Should().Be("UpdTestEntityHandler");
        actual.ViewModel.Should().Be("UpdTestEntityVm");
        actual.Endpoint.Name.Should().Be("UpdTestEntityEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.Should().Be("UpdAsync");
        actual.Endpoint.Route.Should().Be("/testEntity/{id}/upd");
    }

    [Fact]
    public void Should_CustomizeAllAvailableConfiguration() {
        // Arrange
        var sut = CreateFactory(
            new() {
                Generate = false,
                OperationGroup = "CustomOperationGroupName",
                CommandName = "CustomCommandName",
                HandlerName = "CustomHandlerName",
                ViewModelName = "CustomViewModelName",
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
        actual.OperationName.Should().Be("Patch");
        actual.OperationGroup.Should().Be("CustomOperationGroupName");
        actual.Operation.Should().Be("CustomCommandName");
        actual.Handler.Should().Be("CustomHandlerName");
        actual.ViewModel.Should().Be("CustomViewModelName");
        actual.Endpoint.Name.Should().Be("CustomEndpointClassName");
        actual.Endpoint.Generate.Should().BeFalse();
        actual.Endpoint.FunctionName.Should().Be("CustomEndpointFunctionName");
        actual.Endpoint.Route.Should().Be("CustomEndpointRoute");
    }

    private PatchCommandGeneratorRunner CreateFactory(
        InternalEntityGeneratorPatchOperationConfiguration configuration
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