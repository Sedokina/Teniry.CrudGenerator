using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.EntityCustomization;

namespace ITech.CrudGenerator.Tests.BuilderFactories;

public class CreateCommandDefaultConfigurationBuilderFactoryTests
{
    private readonly CreateCommandDefaultConfigurationBuilderFactory _sut;
    private readonly GlobalCqrsGeneratorConfigurationBuilder _globalCqrsGeneratorConfigurationBuilder;
    private readonly CqrsOperationsSharedConfigurationBuilder _cqrsOperationsSharedConfigurationBuilder;

    public CreateCommandDefaultConfigurationBuilderFactoryTests()
    {
        _sut = new CreateCommandDefaultConfigurationBuilderFactory();
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
            new EntityCreateOperationCustomizationScheme());

        // Assert
        actual.GlobalConfiguration.Should().Be(_globalCqrsGeneratorConfigurationBuilder);
        actual.OperationsSharedConfiguration.Should().Be(_cqrsOperationsSharedConfigurationBuilder);
    }

    [Fact]
    public void Should_SetCorrectDefaultValues()
    {
        // Arrange
        var entityName = new EntityName("TestEntity", "TestEntities");
        var operationName = "Create";
        var path = "AllFiles";

        // Act
        var actual = _sut.Construct(
            _globalCqrsGeneratorConfigurationBuilder,
            _cqrsOperationsSharedConfigurationBuilder,
            new EntityCreateOperationCustomizationScheme());

        // Assert
        actual.Generate.Should().BeTrue();
        actual.OperationType.Should().Be(CqrsOperationType.Command);
        actual.OperationName.Should().Be(operationName);
        actual.OperationGroup.GetName(entityName, operationName).Should().Be("CreateTestEntity");
        actual.Operation.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Create.CreateCommand.txt");
        actual.Operation.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("CreateTestEntityCommand");
        actual.Dto.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Create.CreatedDto.txt");
        actual.Dto.NameConfigurationBuilder.GetName(entityName, operationName).Should().Be("CreatedTestEntityDto");
        actual.Handler.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Create.CreateHandler.txt");
        actual.Handler.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("CreateTestEntityHandler");
        actual.Endpoint.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Create.CreateEndpoint.txt");
        actual.Endpoint.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("CreateTestEntityEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.GetName(entityName, operationName).Should().Be("CreateAsync");
        actual.Endpoint.RouteConfigurationBuilder.GetRoute(entityName.Name, operationName, [])
            .Should().Be("/testentity/create");
    }

    [Fact]
    public void Should_CustomizeAllConfigurationWithOperationName_When_OperationNameSetInCustomizationScheme()
    {
        // Arrange
        var entityName = new EntityName("TestEntity", "TestEntities");
        var operationName = "Add";
        var path = "AllFiles";
        var entityCreateOperationCustomizationScheme = new EntityCreateOperationCustomizationScheme
        {
            Operation = operationName
        };

        // Act
        var actual = _sut.Construct(
            _globalCqrsGeneratorConfigurationBuilder,
            _cqrsOperationsSharedConfigurationBuilder,
            entityCreateOperationCustomizationScheme);

        // Assert
        actual.Generate.Should().BeTrue();
        actual.OperationType.Should().Be(CqrsOperationType.Command);
        actual.OperationName.Should().Be(operationName);
        actual.OperationGroup.GetName(entityName, operationName).Should().Be("AddTestEntity");
        actual.Operation.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Create.CreateCommand.txt");
        actual.Operation.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("AddTestEntityCommand");
        actual.Dto.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Create.CreatedDto.txt");
        actual.Dto.NameConfigurationBuilder.GetName(entityName, operationName).Should().Be("CreatedTestEntityDto");
        actual.Handler.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Create.CreateHandler.txt");
        actual.Handler.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("AddTestEntityHandler");
        actual.Endpoint.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Create.CreateEndpoint.txt");
        actual.Endpoint.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("AddTestEntityEndpoint");
        actual.Endpoint.Generate.Should().BeTrue();
        actual.Endpoint.FunctionName.GetName(entityName, operationName).Should().Be("AddAsync");
        actual.Endpoint.RouteConfigurationBuilder.GetRoute(entityName.Name, operationName, [])
            .Should().Be("/testentity/add");
    }
    
     [Fact]
    public void Should_CustomizeAllAvailableConfiguration()
    {
        // Arrange
        var entityName = new EntityName("TestEntity", "TestEntities");
        var operationName = "Create";
        var path = "AllFiles";
        var entityCreateOperationCustomizationScheme = new EntityCreateOperationCustomizationScheme
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
            entityCreateOperationCustomizationScheme);

        // Assert
        actual.Generate.Should().BeFalse();
        actual.OperationType.Should().Be(CqrsOperationType.Command);
        actual.OperationName.Should().Be(operationName);
        actual.OperationGroup.GetName(entityName, operationName).Should().Be("CustomOperationGroupName");
        actual.Operation.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Create.CreateCommand.txt");
        actual.Operation.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("CustomCommandName");
        actual.Dto.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Create.CreatedDto.txt");
        actual.Dto.NameConfigurationBuilder.GetName(entityName, operationName).Should().Be("CustomDtoName");
        actual.Handler.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Create.CreateHandler.txt");
        actual.Handler.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("CustomHandlerName");
        actual.Endpoint.TemplatePath.GetPath(path, "").Should().Be("AllFiles.Create.CreateEndpoint.txt");
        actual.Endpoint.NameConfigurationBuilder.GetName(entityName, operationName)
            .Should().Be("CustomEndpointClassName");
        actual.Endpoint.Generate.Should().BeFalse();
        actual.Endpoint.FunctionName.GetName(entityName, operationName).Should().Be("CustomEndpointFunctionName");
        actual.Endpoint.RouteConfigurationBuilder.GetRoute(entityName.Name, operationName, [])
            .Should().Be("CustomEndpointRoute");
    }
}