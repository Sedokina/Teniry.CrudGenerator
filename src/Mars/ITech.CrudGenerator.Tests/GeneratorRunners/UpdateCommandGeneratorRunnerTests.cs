using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;
using ITech.CrudGenerator.CrudGeneratorCore.GeneratorRunners;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator.Operations;
using ITech.CrudGenerator.Tests.Helpers;
using Microsoft.CodeAnalysis;

namespace ITech.CrudGenerator.Tests.GeneratorRunners;

public class UpdateCommandGeneratorRunnerTests
{
    private readonly GlobalCqrsGeneratorConfigurationBuilder _globalCqrsGeneratorConfigurationBuilder;
    private readonly CqrsOperationsSharedConfigurationBuilder _cqrsOperationsSharedConfigurationBuilder;
    private readonly EntityScheme _entityScheme;

    public UpdateCommandGeneratorRunnerTests()
    {
        _globalCqrsGeneratorConfigurationBuilder = new GlobalCqrsGeneratorConfigurationBuilder();
        _cqrsOperationsSharedConfigurationBuilder = new CqrsOperationsSharedConfigurationBuilderFactory().Construct();
        var internalEntityGeneratorConfiguration = new InternalEntityGeneratorConfiguration(
            new InternalEntityClassMetadata("TestEntity", "", "", [
                new InternalEntityClassPropertyMetadata("Id", "Guid", "Guid", SpecialType.None, true, false)
            ])
        );
        _entityScheme = EntitySchemeFactory.Construct(internalEntityGeneratorConfiguration, new DbContextSchemeStub());
    }

    [Fact]
    public void Should_PutGlobalAndSharedConfigurationsIntoBuiltConfiguration()
    {
        // Arrange
        var sut = CreateFactory(new InternalEntityGeneratorUpdateOperationConfiguration());

        // Act
        var actual = sut.Builder;

        // Assert
        actual.GlobalConfiguration.Should().Be(_globalCqrsGeneratorConfigurationBuilder);
        actual.OperationsSharedConfiguration.Should().Be(_cqrsOperationsSharedConfigurationBuilder);
    }

    [Fact]
    public void Should_SetCorrectDefaultValues()
    {
        // Arrange
        var sut = CreateFactory(new InternalEntityGeneratorUpdateOperationConfiguration());

        // Act
        var actual = sut.Builder.Build(_entityScheme);

        // Assert
        actual.Generate.Should().BeTrue();
        actual.OperationType.Should().Be(CqrsOperationType.Command);
        actual.OperationName.Should().Be("Update");
        actual.OperationGroup.Should().Be("UpdateTestEntity");
        actual.Operation.Should().Be("UpdateTestEntityCommand");
        actual.Handler.Should().Be("UpdateTestEntityHandler");
        actual.ViewModel.Should().Be("UpdateTestEntityVm");
        actual.Endpoint.Name.Should().Be("UpdateTestEntityEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.Should().Be("UpdateAsync");
        actual.Endpoint.Route.Should().Be("/testEntity/{id}/update");
    }

    [Fact]
    public void Should_CustomizeAllConfigurationWithOperationName_When_OperationNameSetInGeneratorConfiguration()
    {
        // Arrange
        var sut = CreateFactory(new InternalEntityGeneratorUpdateOperationConfiguration
        {
            Operation = "Upd"
        });

        // Act
        var actual = sut.Builder.Build(_entityScheme);

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
    public void Should_CustomizeAllAvailableConfiguration()
    {
        // Arrange
        var sut = CreateFactory(new InternalEntityGeneratorUpdateOperationConfiguration
        {
            Generate = false,
            OperationGroup = "CustomOperationGroupName",
            CommandName = "CustomCommandName",
            HandlerName = "CustomHandlerName",
            ViewModelName = "CustomViewModelName",
            EndpointClassName = "CustomEndpointClassName",
            EndpointFunctionName = "CustomEndpointFunctionName",
            GenerateEndpoint = false,
            RouteName = "CustomEndpointRoute"
        });

        // Act
        var actual = sut.Builder.Build(_entityScheme);

        // Assert
        actual.Generate.Should().BeFalse();
        actual.OperationType.Should().Be(CqrsOperationType.Command);
        actual.OperationName.Should().Be("Update");
        actual.OperationGroup.Should().Be("CustomOperationGroupName");
        actual.Operation.Should().Be("CustomCommandName");
        actual.Handler.Should().Be("CustomHandlerName");
        actual.ViewModel.Should().Be("CustomViewModelName");
        actual.Endpoint.Name.Should().Be("CustomEndpointClassName");
        actual.Endpoint.Generate.Should().BeFalse();
        actual.Endpoint.FunctionName.Should().Be("CustomEndpointFunctionName");
        actual.Endpoint.Route.Should().Be("CustomEndpointRoute");
    }

    private UpdateCommandGeneratorRunner CreateFactory(
        InternalEntityGeneratorUpdateOperationConfiguration configuration)
    {
        return new UpdateCommandGeneratorRunner(
            _globalCqrsGeneratorConfigurationBuilder,
            _cqrsOperationsSharedConfigurationBuilder,
            configuration,
            _entityScheme,
            new DbContextSchemeStub()
        );
    }
}