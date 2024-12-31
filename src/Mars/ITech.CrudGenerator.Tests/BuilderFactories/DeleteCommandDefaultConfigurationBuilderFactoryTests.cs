using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator.Operations;
using ITech.CrudGenerator.Tests.Helpers;

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
        _globalCqrsGeneratorConfigurationBuilder = new GlobalCqrsGeneratorConfigurationBuilder
            { TemplatesBasePath = "AllFiles" };
        _cqrsOperationsSharedConfigurationBuilder = new CqrsOperationsSharedConfigurationBuilderFactory().Construct();
        var entitySchemeFactory = new EntitySchemeFactory();
        var symbol = DynamicClassBuilder.GenerateEntity("TestEntity", "public Guid Id {{ get; set; }}");
        _entityScheme = entitySchemeFactory.Construct(symbol, new InternalEntityGeneratorConfiguration(),
            new DbContextSchemeStub());
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
        actual.Operation.TemplatePath.Should().Be("AllFiles.Delete.DeleteCommand.txt");
        actual.Operation.Name.Should().Be("DeleteTestEntityCommand");
        actual.Handler.Name.Should().Be("DeleteTestEntityHandler");
        actual.Endpoint.TemplatePath.Should().Be("AllFiles.Delete.DeleteEndpoint.txt");
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
        actual.Operation.TemplatePath.Should().Be("AllFiles.Delete.DeleteCommand.txt");
        actual.Operation.Name.Should().Be("DelTestEntityCommand");
        actual.Handler.Name.Should().Be("DelTestEntityHandler");
        actual.Endpoint.TemplatePath.Should().Be("AllFiles.Delete.DeleteEndpoint.txt");
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
        actual.Operation.TemplatePath.Should().Be("AllFiles.Delete.DeleteCommand.txt");
        actual.Operation.Name.Should().Be("CustomCommandName");
        actual.Handler.Name.Should().Be("CustomHandlerName");
        actual.Endpoint.TemplatePath.Should().Be("AllFiles.Delete.DeleteEndpoint.txt");
        actual.Endpoint.Name.Should().Be("CustomEndpointClassName");
        actual.Endpoint.Generate.Should().BeFalse();
        actual.Endpoint.FunctionName.Should().Be("CustomEndpointFunctionName");
        actual.Endpoint.Route.Should().Be("CustomEndpointRoute");
    }
}