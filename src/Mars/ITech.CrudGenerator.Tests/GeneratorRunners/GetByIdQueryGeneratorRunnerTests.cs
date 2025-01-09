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

public class GetByIdQueryGeneratorRunnerTests
{
    private readonly GetByIdQueryGeneratorRunner _sut;
    private readonly GlobalCqrsGeneratorConfigurationBuilder _globalCqrsGeneratorConfigurationBuilder;
    private readonly CqrsOperationsSharedConfigurationBuilder _cqrsOperationsSharedConfigurationBuilder;
    private readonly EntityScheme _entityScheme;

    public GetByIdQueryGeneratorRunnerTests()
    {
        _globalCqrsGeneratorConfigurationBuilder = new GlobalCqrsGeneratorConfigurationBuilder();
        _cqrsOperationsSharedConfigurationBuilder = new CqrsOperationsSharedConfigurationBuilderFactory().Construct();
        var internalEntityGeneratorConfiguration = new InternalEntityGeneratorConfiguration(
            new InternalEntityClassMetadata("TestEntity", "", "", [
                new InternalEntityClassPropertyMetadata("Id", "Guid", "Guid", SpecialType.None, true, false)
            ])
        );
        _entityScheme = EntitySchemeFactory.Construct(internalEntityGeneratorConfiguration, new DbContextSchemeStub());
        _sut = new GetByIdQueryGeneratorRunner(_globalCqrsGeneratorConfigurationBuilder,
            _cqrsOperationsSharedConfigurationBuilder,
            internalEntityGeneratorConfiguration.GetByIdOperation,
            _entityScheme,
            new DbContextSchemeStub());
    }

    [Fact]
    public void Should_PutGlobalAndSharedConfigurationsIntoBuiltConfiguration()
    {
        // Arrange
        var sut = CreateFactory(new InternalEntityGeneratorGetByIdOperationConfiguration());

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
        var sut = CreateFactory(new InternalEntityGeneratorGetByIdOperationConfiguration());

        // Act
        var actual = sut.Builder.Build(_entityScheme);

        // Assert
        actual.Generate.Should().BeTrue();
        actual.OperationType.Should().Be(CqrsOperationType.Query);
        actual.OperationName.Should().Be("Get");
        actual.OperationGroup.Should().Be("GetTestEntity");
        actual.Operation.Should().Be("GetTestEntityQuery");
        actual.Dto.Should().Be("TestEntityDto");
        actual.Handler.Should().Be("GetTestEntityHandler");
        actual.Endpoint.Name.Should().Be("GetTestEntityEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.Should().Be("GetAsync");
        actual.Endpoint.Route.Should().Be("/testEntity/{id}");
    }

    [Fact]
    public void Should_CustomizeAllConfigurationWithOperationName_When_OperationNameSetInGeneratorConfiguration()
    {
        // Arrange
        var sut = CreateFactory(new InternalEntityGeneratorGetByIdOperationConfiguration
        {
            Operation = "Obtain"
        });

        // Act
        var actual = sut.Builder.Build(_entityScheme);

        // Assert
        actual.Generate.Should().BeTrue();
        actual.OperationType.Should().Be(CqrsOperationType.Query);
        actual.OperationName.Should().Be("Obtain");
        actual.OperationGroup.Should().Be("ObtainTestEntity");
        actual.Operation.Should().Be("ObtainTestEntityQuery");
        actual.Dto.Should().Be("TestEntityDto");
        actual.Handler.Should().Be("ObtainTestEntityHandler");
        actual.Endpoint.Name.Should().Be("ObtainTestEntityEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.Should().Be("ObtainAsync");
        actual.Endpoint.Route.Should().Be("/testEntity/{id}");
    }

    [Fact]
    public void Should_CustomizeAllAvailableConfiguration()
    {
        // Arrange
        var sut = CreateFactory(new InternalEntityGeneratorGetByIdOperationConfiguration
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
        var actual = sut.Builder.Build(_entityScheme);

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

    private GetByIdQueryGeneratorRunner CreateFactory(
        InternalEntityGeneratorGetByIdOperationConfiguration configuration)
    {
        return new GetByIdQueryGeneratorRunner(
            _globalCqrsGeneratorConfigurationBuilder,
            _cqrsOperationsSharedConfigurationBuilder,
            configuration,
            _entityScheme,
            new DbContextSchemeStub()
        );
    }
}