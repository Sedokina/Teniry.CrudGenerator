using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator.Operations;
using ITech.CrudGenerator.Tests.Helpers;

namespace ITech.CrudGenerator.Tests.BuilderFactories;

public class UpdateCommandDefaultConfigurationBuilderFactoryTests
{
    private readonly UpdateCommandDefaultConfigurationBuilderFactory _sut;
    private readonly GlobalCqrsGeneratorConfigurationBuilder _globalCqrsGeneratorConfigurationBuilder;
    private readonly CqrsOperationsSharedConfigurationBuilder _cqrsOperationsSharedConfigurationBuilder;
    private readonly EntityScheme _entityScheme;

    public UpdateCommandDefaultConfigurationBuilderFactoryTests()
    {
        _sut = new UpdateCommandDefaultConfigurationBuilderFactory();
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
            new InternalEntityGeneratorUpdateOperationConfiguration());

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
                new InternalEntityGeneratorUpdateOperationConfiguration())
            .Build(_entityScheme);

        // Assert
        actual.Generate.Should().BeTrue();
        actual.OperationType.Should().Be(CqrsOperationType.Command);
        actual.OperationName.Should().Be("Update");
        actual.OperationGroup.Should().Be("UpdateTestEntity");
        actual.Operation.TemplatePath.Should().Be("AllFiles.Update.UpdateCommand.txt");
        actual.Operation.Name.Should().Be("UpdateTestEntityCommand");
        actual.Handler.TemplatePath.Should().Be("AllFiles.Update.UpdateHandler.txt");
        actual.Handler.Name.Should().Be("UpdateTestEntityHandler");
        actual.ViewModel.TemplatePath.Should().Be("AllFiles.Update.UpdateVm.txt");
        actual.ViewModel.Name.Should().Be("UpdateTestEntityVm");
        actual.Endpoint.TemplatePath.Should().Be("AllFiles.Update.UpdateEndpoint.txt");
        actual.Endpoint.Name.Should().Be("UpdateTestEntityEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.Should().Be("UpdateAsync");
        actual.Endpoint.Route.Should().Be("/testEntity/{id}/update");
    }

    [Fact]
    public void Should_CustomizeAllConfigurationWithOperationName_When_OperationNameSetInGeneratorConfiguration()
    {
        // Arrange
        var operationConfiguration = new InternalEntityGeneratorUpdateOperationConfiguration
        {
            Operation = "Upd"
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
        actual.OperationName.Should().Be("Upd");
        actual.OperationGroup.Should().Be("UpdTestEntity");
        actual.Operation.TemplatePath.Should().Be("AllFiles.Update.UpdateCommand.txt");
        actual.Operation.Name.Should().Be("UpdTestEntityCommand");
        actual.Handler.TemplatePath.Should().Be("AllFiles.Update.UpdateHandler.txt");
        actual.Handler.Name.Should().Be("UpdTestEntityHandler");
        actual.ViewModel.TemplatePath.Should().Be("AllFiles.Update.UpdateVm.txt");
        actual.ViewModel.Name.Should().Be("UpdTestEntityVm");
        actual.Endpoint.TemplatePath.Should().Be("AllFiles.Update.UpdateEndpoint.txt");
        actual.Endpoint.Name.Should().Be("UpdTestEntityEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.Should().Be("UpdAsync");
        actual.Endpoint.Route.Should().Be("/testEntity/{id}/upd");
    }

    [Fact]
    public void Should_CustomizeAllAvailableConfiguration()
    {
        // Arrange
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
        var actual = _sut
            .Construct(
                _globalCqrsGeneratorConfigurationBuilder,
                _cqrsOperationsSharedConfigurationBuilder,
                operationConfiguration)
            .Build(_entityScheme);

        // Assert
        actual.Generate.Should().BeFalse();
        actual.OperationType.Should().Be(CqrsOperationType.Command);
        actual.OperationName.Should().Be("Update");
        actual.OperationGroup.Should().Be("CustomOperationGroupName");
        actual.Operation.TemplatePath.Should().Be("AllFiles.Update.UpdateCommand.txt");
        actual.Operation.Name.Should().Be("CustomCommandName");
        actual.Handler.TemplatePath.Should().Be("AllFiles.Update.UpdateHandler.txt");
        actual.Handler.Name.Should().Be("CustomHandlerName");
        actual.ViewModel.TemplatePath.Should().Be("AllFiles.Update.UpdateVm.txt");
        actual.ViewModel.Name.Should().Be("CustomViewModelName");
        actual.Endpoint.TemplatePath.Should().Be("AllFiles.Update.UpdateEndpoint.txt");
        actual.Endpoint.Name.Should().Be("CustomEndpointClassName");
        actual.Endpoint.Generate.Should().BeFalse();
        actual.Endpoint.FunctionName.Should().Be("CustomEndpointFunctionName");
        actual.Endpoint.Route.Should().Be("CustomEndpointRoute");
    }
}