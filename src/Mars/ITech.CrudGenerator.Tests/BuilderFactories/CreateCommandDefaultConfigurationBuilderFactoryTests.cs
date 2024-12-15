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

public class CreateCommandDefaultConfigurationBuilderFactoryTests
{
    private readonly CreateCommandDefaultConfigurationBuilderFactory _sut;
    private readonly GlobalCqrsGeneratorConfigurationBuilder _globalCqrsGeneratorConfigurationBuilder;
    private readonly CqrsOperationsSharedConfigurationBuilder _cqrsOperationsSharedConfigurationBuilder;
    private readonly EntityScheme _entityScheme;

    public CreateCommandDefaultConfigurationBuilderFactoryTests()
    {
        _sut = new CreateCommandDefaultConfigurationBuilderFactory();
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
            new InternalEntityGeneratorCreateOperationConfiguration());

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
                new InternalEntityGeneratorCreateOperationConfiguration())
            .Build(_entityScheme);

        // Assert
        actual.Generate.Should().BeTrue();
        actual.OperationType.Should().Be(CqrsOperationType.Command);
        actual.OperationName.Should().Be("Create");
        actual.OperationGroup.Should().Be("CreateTestEntity");
        actual.Operation.TemplatePath.Should().Be("AllFiles.Create.CreateCommand.txt");
        actual.Operation.Name.Should().Be("CreateTestEntityCommand");
        actual.Dto.TemplatePath.Should().Be("AllFiles.Create.CreatedDto.txt");
        actual.Dto.Name.Should().Be("CreatedTestEntityDto");
        actual.Handler.TemplatePath.Should().Be("AllFiles.Create.CreateHandler.txt");
        actual.Handler.Name.Should().Be("CreateTestEntityHandler");
        actual.Endpoint.TemplatePath.Should().Be("AllFiles.Create.CreateEndpoint.txt");
        actual.Endpoint.Name.Should().Be("CreateTestEntityEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.Should().Be("CreateAsync");
        actual.Endpoint.Route.Should().Be("/testEntity/create");
    }

    [Fact]
    public void Should_CustomizeAllConfigurationWithOperationName_When_OperationNameSetInGeneratorConfiguration()
    {
        // Arrange
        var operationConfiguration = new InternalEntityGeneratorCreateOperationConfiguration
        {
            Operation = "Add"
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
        actual.OperationName.Should().Be("Add");
        actual.OperationGroup.Should().Be("AddTestEntity");
        actual.Operation.TemplatePath.Should().Be("AllFiles.Create.CreateCommand.txt");
        actual.Operation.Name.Should().Be("AddTestEntityCommand");
        actual.Dto.TemplatePath.Should().Be("AllFiles.Create.CreatedDto.txt");
        actual.Dto.Name.Should().Be("CreatedTestEntityDto");
        actual.Handler.TemplatePath.Should().Be("AllFiles.Create.CreateHandler.txt");
        actual.Handler.Name.Should().Be("AddTestEntityHandler");
        actual.Endpoint.TemplatePath.Should().Be("AllFiles.Create.CreateEndpoint.txt");
        actual.Endpoint.Name.Should().Be("AddTestEntityEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.Should().Be("AddAsync");
        actual.Endpoint.Route.Should().Be("/testEntity/add");
    }

    [Fact]
    public void Should_CustomizeAllAvailableConfiguration()
    {
        // Arrange
        var operationConfiguration = new InternalEntityGeneratorCreateOperationConfiguration
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
        };

        // Act
        var actual = _sut.Construct(
                _globalCqrsGeneratorConfigurationBuilder,
                _cqrsOperationsSharedConfigurationBuilder,
                operationConfiguration)
            .Build(_entityScheme);

        // Assert
        actual.Generate.Should().BeFalse();
        actual.OperationType.Should().Be(CqrsOperationType.Command);
        actual.OperationName.Should().Be("Create");
        actual.OperationGroup.Should().Be("CustomOperationGroupName");
        actual.Operation.TemplatePath.Should().Be("AllFiles.Create.CreateCommand.txt");
        actual.Operation.Name.Should().Be("CustomCommandName");
        actual.Dto.TemplatePath.Should().Be("AllFiles.Create.CreatedDto.txt");
        actual.Dto.Name.Should().Be("CustomDtoName");
        actual.Handler.TemplatePath.Should().Be("AllFiles.Create.CreateHandler.txt");
        actual.Handler.Name.Should().Be("CustomHandlerName");
        actual.Endpoint.TemplatePath.Should().Be("AllFiles.Create.CreateEndpoint.txt");
        actual.Endpoint.Name.Should().Be("CustomEndpointClassName");
        actual.Endpoint.Generate.Should().BeFalse();
        actual.Endpoint.FunctionName.Should().Be("CustomEndpointFunctionName");
        actual.Endpoint.Route.Should().Be("CustomEndpointRoute");
    }
}