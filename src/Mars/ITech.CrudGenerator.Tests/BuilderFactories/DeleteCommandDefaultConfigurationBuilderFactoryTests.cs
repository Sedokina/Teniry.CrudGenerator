using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator.Operations;
using ITech.CrudGenerator.Tests.Helpers;
using Microsoft.CodeAnalysis;

namespace ITech.CrudGenerator.Tests.BuilderFactories;

public class DeleteCommandDefaultConfigurationBuilderFactoryTests
{
    private readonly DeleteCommandDefaultConfigurationBuilderFactory _sut;
    private readonly GlobalCqrsGeneratorConfigurationBuilder _globalCqrsGeneratorConfigurationBuilder;
    private readonly CqrsOperationsSharedConfigurationBuilder _cqrsOperationsSharedConfigurationBuilder;
    private readonly EntityScheme _entityScheme;

    public DeleteCommandDefaultConfigurationBuilderFactoryTests()
    {
        _sut = new DeleteCommandDefaultConfigurationBuilderFactory();
        _globalCqrsGeneratorConfigurationBuilder = new GlobalCqrsGeneratorConfigurationBuilder();
        _cqrsOperationsSharedConfigurationBuilder = new CqrsOperationsSharedConfigurationBuilderFactory().Construct();
        var internalEntityGeneratorConfiguration = new InternalEntityGeneratorConfiguration(
            new InternalEntityClassMetadata("TestEntity", "", "", [
                new InternalEntityClassPropertyMetadata("Id", "Guid", "Guid", SpecialType.None, true, false)
            ])
        );
        var entitySchemeFactory = new EntitySchemeFactory();
        _entityScheme = entitySchemeFactory.Construct(internalEntityGeneratorConfiguration, new DbContextSchemeStub());
    }

    [Fact]
    public void Should_PutGlobalAndSharedConfigurationsIntoBuiltConfiguration()
    {
        // Act
        var actual = _sut.Construct(
            _globalCqrsGeneratorConfigurationBuilder,
            _cqrsOperationsSharedConfigurationBuilder,
            new InternalEntityGeneratorDeleteOperationConfiguration());

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
                new InternalEntityGeneratorDeleteOperationConfiguration())
            .Build(_entityScheme);

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
    public void Should_CustomizeAllConfigurationWithOperationName_When_OperationNameSetInGeneratorConfiguration()
    {
        // Arrange
        var operationConfiguration = new InternalEntityGeneratorDeleteOperationConfiguration
        {
            Operation = "Del"
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
    public void Should_CustomizeAllAvailableConfiguration()
    {
        // Arrange
        var operationConfiguration = new InternalEntityGeneratorDeleteOperationConfiguration
        {
            Generate = false,
            OperationGroup = "CustomOperationGroupName",
            CommandName = "CustomCommandName",
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
}