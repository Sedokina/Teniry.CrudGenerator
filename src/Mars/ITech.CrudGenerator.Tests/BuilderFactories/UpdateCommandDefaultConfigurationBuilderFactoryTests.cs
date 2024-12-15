using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator.Operations;

namespace ITech.CrudGenerator.Tests.BuilderFactories;

public class UpdateCommandDefaultConfigurationBuilderFactoryTests
{
    private readonly UpdateCommandDefaultConfigurationBuilderFactory _sut;
    private readonly GlobalCqrsGeneratorConfigurationBuilder _globalCqrsGeneratorConfigurationBuilder;
    private readonly CqrsOperationsSharedConfigurationBuilder _cqrsOperationsSharedConfigurationBuilder;

    public UpdateCommandDefaultConfigurationBuilderFactoryTests()
    {
        _sut = new UpdateCommandDefaultConfigurationBuilderFactory();
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
            new InternalEntityGeneratorUpdateOperationConfiguration());

        // Assert
        actual.GlobalConfiguration.Should().Be(_globalCqrsGeneratorConfigurationBuilder);
        actual.OperationsSharedConfiguration.Should().Be(_cqrsOperationsSharedConfigurationBuilder);
    }

    [Fact]
    public void Should_SetCorrectDefaultValues()
    {
        // Arrange
        var entityName = new EntityName("TestEntity", "TestEntities");
        var operationName = "Update";
        var path = "AllFiles";

        // Act
        var actual = _sut.Construct(
            _globalCqrsGeneratorConfigurationBuilder,
            _cqrsOperationsSharedConfigurationBuilder,
            new InternalEntityGeneratorUpdateOperationConfiguration());

        // Assert
        actual.Generate.Should().BeTrue();
        actual.OperationType.Should().Be(CqrsOperationType.Command);
        actual.OperationName.Should().Be(operationName);
        actual.OperationGroup.GetName(entityName, operationName).Should().Be("UpdateTestEntity");
        actual.Operation.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Update.UpdateCommand.txt");
        actual.Operation.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("UpdateTestEntityCommand");
        actual.Handler.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Update.UpdateHandler.txt");
        actual.Handler.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("UpdateTestEntityHandler");
        actual.ViewModel.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Update.UpdateVm.txt");
        actual.ViewModel.NameConfigurationBuilder.GetName(entityName, operationName).Should().Be("UpdateTestEntityVm");
        actual.Endpoint.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Update.UpdateEndpoint.txt");
        actual.Endpoint.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("UpdateTestEntityEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.GetName(entityName, operationName).Should().Be("UpdateAsync");
        actual.Endpoint.RouteConfigurationBuilder.GetRoute(entityName.Name, operationName, ["id"])
            .Should().Be("/testentity/{id}/update");
    }

    [Fact]
    public void Should_CustomizeAllConfigurationWithOperationName_When_OperationNameSetInGeneratorConfiguration()
    {
        // Arrange
        var entityName = new EntityName("TestEntity", "TestEntities");
        var operationName = "Upd";
        var path = "AllFiles";
        var operationConfiguration = new InternalEntityGeneratorUpdateOperationConfiguration
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
        actual.OperationGroup.GetName(entityName, operationName).Should().Be("UpdTestEntity");
        actual.Operation.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Update.UpdateCommand.txt");
        actual.Operation.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("UpdTestEntityCommand");
        actual.Handler.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Update.UpdateHandler.txt");
        actual.Handler.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("UpdTestEntityHandler");
        actual.ViewModel.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Update.UpdateVm.txt");
        actual.ViewModel.NameConfigurationBuilder.GetName(entityName, operationName).Should().Be("UpdTestEntityVm");
        actual.Endpoint.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Update.UpdateEndpoint.txt");
        actual.Endpoint.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("UpdTestEntityEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.GetName(entityName, operationName).Should().Be("UpdAsync");
        actual.Endpoint.RouteConfigurationBuilder.GetRoute(entityName.Name, operationName, ["id"])
            .Should().Be("/testentity/{id}/upd");
    }

    [Fact]
    public void Should_CustomizeAllAvailableConfiguration()
    {
        // Arrange
        var entityName = new EntityName("TestEntity", "TestEntities");
        var operationName = "Update";
        var path = "AllFiles";
        var operationConfiguration = new InternalEntityGeneratorUpdateOperationConfiguration
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
        actual.Operation.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Update.UpdateCommand.txt");
        actual.Operation.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("CustomCommandName");
        actual.Handler.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Update.UpdateHandler.txt");
        actual.Handler.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("CustomHandlerName");
        actual.ViewModel.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Update.UpdateVm.txt");
        actual.ViewModel.NameConfigurationBuilder.GetName(entityName, operationName).Should().Be("CustomViewModelName");
        actual.Endpoint.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Update.UpdateEndpoint.txt");
        actual.Endpoint.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("CustomEndpointClassName");
        actual.Endpoint.Generate.Should().BeFalse();
        actual.Endpoint.FunctionName.GetName(entityName, operationName).Should().Be("CustomEndpointFunctionName");
        actual.Endpoint.RouteConfigurationBuilder.GetRoute(entityName.Name, operationName, [])
            .Should().Be("CustomEndpointRoute");
    }
}