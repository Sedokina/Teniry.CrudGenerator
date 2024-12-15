using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator;

namespace ITech.CrudGenerator.Tests.BuilderFactories;

public class DeleteCommandDefaultConfigurationBuilderFactoryTests
{
    private readonly DeleteCommandDefaultConfigurationBuilderFactory _sut;
    private readonly GlobalCqrsGeneratorConfigurationBuilder _globalCqrsGeneratorConfigurationBuilder;
    private readonly CqrsOperationsSharedConfigurationBuilder _cqrsOperationsSharedConfigurationBuilder;

    public DeleteCommandDefaultConfigurationBuilderFactoryTests()
    {
        _sut = new DeleteCommandDefaultConfigurationBuilderFactory();
        _globalCqrsGeneratorConfigurationBuilder = new GlobalCqrsGeneratorConfigurationBuilder();
        _cqrsOperationsSharedConfigurationBuilder = new CqrsOperationsSharedConfigurationBuilder();
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
        // Arrange
        var entityName = new EntityName("TestEntity", "TestEntities");
        var operationName = "Delete";
        var path = "AllFiles";

        // Act
        var actual = _sut.Construct(
            _globalCqrsGeneratorConfigurationBuilder,
            _cqrsOperationsSharedConfigurationBuilder,
            new InternalEntityGeneratorDeleteOperationConfiguration());

        // Assert
        actual.Generate.Should().BeTrue();
        actual.OperationType.Should().Be(CqrsOperationType.Command);
        actual.OperationName.Should().Be(operationName);
        actual.OperationGroup.GetName(entityName, operationName).Should().Be("DeleteTestEntity");
        actual.Operation.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Delete.DeleteCommand.txt");
        actual.Operation.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("DeleteTestEntityCommand");
        actual.Handler.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Delete.DeleteHandler.txt");
        actual.Handler.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("DeleteTestEntityHandler");
        actual.Endpoint.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Delete.DeleteEndpoint.txt");
        actual.Endpoint.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("DeleteTestEntityEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.GetName(entityName, operationName).Should().Be("DeleteAsync");
        actual.Endpoint.RouteConfigurationBuilder.GetRoute(entityName.Name, operationName, ["id"])
            .Should().Be("/testentity/{id}/delete");
    }

    [Fact]
    public void Should_CustomizeAllConfigurationWithOperationName_When_OperationNameSetInGeneratorConfiguration()
    {
        // Arrange
        var entityName = new EntityName("TestEntity", "TestEntities");
        var operationName = "Del";
        var path = "AllFiles";
        var operationConfiguration = new InternalEntityGeneratorDeleteOperationConfiguration
        {
            Operation = operationName
        };

        // Act
        var actual = _sut.Construct(
            _globalCqrsGeneratorConfigurationBuilder,
            _cqrsOperationsSharedConfigurationBuilder,
            operationConfiguration);

        // Assert
        actual.Generate.Should().BeTrue();
        actual.OperationType.Should().Be(CqrsOperationType.Command);
        actual.OperationName.Should().Be(operationName);
        actual.OperationGroup.GetName(entityName, operationName).Should().Be("DelTestEntity");
        actual.Operation.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Delete.DeleteCommand.txt");
        actual.Operation.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("DelTestEntityCommand");
        actual.Handler.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Delete.DeleteHandler.txt");
        actual.Handler.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("DelTestEntityHandler");
        actual.Endpoint.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Delete.DeleteEndpoint.txt");
        actual.Endpoint.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("DelTestEntityEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.GetName(entityName, operationName).Should().Be("DelAsync");
        actual.Endpoint.RouteConfigurationBuilder.GetRoute(entityName.Name, operationName, ["id"])
            .Should().Be("/testentity/{id}/del");
    }
    
     [Fact]
    public void Should_CustomizeAllAvailableConfiguration()
    {
        // Arrange
        var entityName = new EntityName("TestEntity", "TestEntities");
        var operationName = "Delete";
        var path = "AllFiles";
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
        var actual = _sut.Construct(
            _globalCqrsGeneratorConfigurationBuilder,
            _cqrsOperationsSharedConfigurationBuilder,
            operationConfiguration);

        // Assert
        actual.Generate.Should().BeFalse();
        actual.OperationType.Should().Be(CqrsOperationType.Command);
        actual.OperationName.Should().Be(operationName);
        actual.OperationGroup.GetName(entityName, operationName).Should().Be("CustomOperationGroupName");
        actual.Operation.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Delete.DeleteCommand.txt");
        actual.Operation.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("CustomCommandName");
        actual.Handler.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Delete.DeleteHandler.txt");
        actual.Handler.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("CustomHandlerName");
        actual.Endpoint.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Delete.DeleteEndpoint.txt");
        actual.Endpoint.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("CustomEndpointClassName");
        actual.Endpoint.Generate.Should().BeFalse();
        actual.Endpoint.FunctionName.GetName(entityName, operationName).Should().Be("CustomEndpointFunctionName");
        actual.Endpoint.RouteConfigurationBuilder.GetRoute(entityName.Name, operationName, [])
            .Should().Be("CustomEndpointRoute");
    }
}