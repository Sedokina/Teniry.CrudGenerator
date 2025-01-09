using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;
using ITech.CrudGenerator.CrudGeneratorCore.GeneratorRunners;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator.Operations;
using ITech.CrudGenerator.Tests.Helpers;

namespace ITech.CrudGenerator.Tests.GeneratorRunners;

public class CreateCommandGeneratorRunnerTests
{
    private readonly GlobalCqrsGeneratorConfigurationBuilder _globalCqrsGeneratorConfigurationBuilder;
    private readonly CqrsOperationsSharedConfigurationBuilder _cqrsOperationsSharedConfigurationBuilder;
    private readonly EntityScheme _entityScheme;

    public CreateCommandGeneratorRunnerTests()
    {
        _globalCqrsGeneratorConfigurationBuilder = new GlobalCqrsGeneratorConfigurationBuilder();
        _cqrsOperationsSharedConfigurationBuilder = new CqrsOperationsSharedConfigurationBuilderFactory().Construct();
        var internalEntityGeneratorConfiguration =
            new InternalEntityGeneratorConfiguration(new InternalEntityClassMetadata("TestEntity", "", "", []));
        var entitySchemeFactory = new EntitySchemeFactory();
        _entityScheme = entitySchemeFactory.Construct(internalEntityGeneratorConfiguration, new DbContextSchemeStub());
    }

    [Fact]
    public void Should_PutGlobalAndSharedConfigurationsIntoBuiltConfiguration()
    {
        // Arrange
        var sut = CreateFactory(new InternalEntityGeneratorCreateOperationConfiguration());

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
        var sut = CreateFactory(new InternalEntityGeneratorCreateOperationConfiguration());

        // Act
        var actual = sut.Builder.Build(_entityScheme);

        // Assert
        actual.Generate.Should().BeTrue();
        actual.OperationType.Should().Be(CqrsOperationType.Command);
        actual.OperationName.Should().Be("Create");
        actual.OperationGroup.Should().Be("CreateTestEntity");
        actual.Operation.Should().Be("CreateTestEntityCommand");
        actual.Dto.Should().Be("CreatedTestEntityDto");
        actual.Handler.Should().Be("CreateTestEntityHandler");
        actual.Endpoint.Name.Should().Be("CreateTestEntityEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.Should().Be("CreateAsync");
        actual.Endpoint.Route.Should().Be("/testEntity/create");
    }

    [Fact]
    public void Should_CustomizeAllConfigurationWithOperationName_When_OperationNameSetInGeneratorConfiguration()
    {
        // Arrange
        var sut = CreateFactory(new InternalEntityGeneratorCreateOperationConfiguration
        {
            Operation = "Add"
        });

        // Act
        var actual = sut.Builder.Build(_entityScheme);

        // Assert
        actual.Generate.Should().BeTrue();
        actual.OperationType.Should().Be(CqrsOperationType.Command);
        actual.OperationName.Should().Be("Add");
        actual.OperationGroup.Should().Be("AddTestEntity");
        actual.Operation.Should().Be("AddTestEntityCommand");
        actual.Dto.Should().Be("CreatedTestEntityDto");
        actual.Handler.Should().Be("AddTestEntityHandler");
        actual.Endpoint.Name.Should().Be("AddTestEntityEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.Should().Be("AddAsync");
        actual.Endpoint.Route.Should().Be("/testEntity/add");
    }

    [Fact]
    public void Should_CustomizeAllAvailableConfiguration()
    {
        // Arrange
        var sut = CreateFactory(new InternalEntityGeneratorCreateOperationConfiguration
        {
            Generate = false,
            OperationGroup = "CustomOperationGroupName",
            CommandName = "CustomCommandName",
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
        actual.OperationType.Should().Be(CqrsOperationType.Command);
        actual.OperationName.Should().Be("Create");
        actual.OperationGroup.Should().Be("CustomOperationGroupName");
        actual.Operation.Should().Be("CustomCommandName");
        actual.Dto.Should().Be("CustomDtoName");
        actual.Handler.Should().Be("CustomHandlerName");
        actual.Endpoint.Name.Should().Be("CustomEndpointClassName");
        actual.Endpoint.Generate.Should().BeFalse();
        actual.Endpoint.FunctionName.Should().Be("CustomEndpointFunctionName");
        actual.Endpoint.Route.Should().Be("CustomEndpointRoute");
    }


    private CreateCommandGeneratorRunner CreateFactory(
        InternalEntityGeneratorCreateOperationConfiguration configuration)
    {
        return new CreateCommandGeneratorRunner(
            _globalCqrsGeneratorConfigurationBuilder,
            _cqrsOperationsSharedConfigurationBuilder,
            configuration,
            _entityScheme,
            new DbContextSchemeStub(),
            null!,
            null!
        );
    }
}