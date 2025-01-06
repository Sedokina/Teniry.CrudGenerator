using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global.Factories;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator.Operations;
using ITech.CrudGenerator.Tests.Helpers;

namespace ITech.CrudGenerator.Tests;

public class CreateCommandCrudGeneratorTests
{
    private readonly CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration> _crudGeneratorScheme;

    public CreateCommandCrudGeneratorTests()
    {
        var globalCqrsGeneratorConfigurationBuilder =
            GlobalCrudGeneratorConfigurationDefaultConfigurationFactory.Construct();
        var cqrsOperationsSharedConfigurationBuilder =
            new CqrsOperationsSharedConfigurationBuilderFactory().Construct();
        var entitySchemeFactory = new EntitySchemeFactory();
        var symbol = DynamicClassBuilder.GenerateEntity("TestEntity",
            "public Guid Id {{ get; set; }}\npublic string Name {{ get; set; }}");
        var entityScheme = entitySchemeFactory.Construct(symbol, new InternalEntityGeneratorConfiguration(),
            new DbContextSchemeStub());

        var configuration = new CreateCommandDefaultConfigurationBuilderFactory()
            .Construct(
                globalCqrsGeneratorConfigurationBuilder,
                cqrsOperationsSharedConfigurationBuilder,
                new InternalEntityGeneratorCreateOperationConfiguration())
            .Build(entityScheme);
        _crudGeneratorScheme = new CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration>(entityScheme,
            new DbContextSchemeStub(),
            configuration);
    }

    [Fact]
    public void Should_ReturnGetRouteFromCreateEndpoint()
    {
        // Arrange
        var getByIdEndpointRouteConfigurationBuilder =
            new EndpointRouteConfigurationBuilder("mygetroute/{{id_param_name}}");
        var sut = new CreateCommandCrudGenerator(_crudGeneratorScheme, getByIdEndpointRouteConfigurationBuilder,
            "getbyid");

        // Act
        sut.RunGenerator();

        // Assert
        var endpoint = sut.GeneratedFiles.Find(x => x.FileName.Equals("CreateTestEntityEndpoint.g.cs"));
        endpoint.Should().NotBeNull();
        endpoint!.Source.ToString().Should().Contain(@"var resourceRoute = $""mygetroute/{result.Id}""");
        endpoint.Source.ToString().Should().Contain("return TypedResults.Created(resourceRoute, result);");
    }

    [Fact]
    public void
        Should_NotReturnGetRouteFromCreateEndpoint_When_GetRouteConfigurationNotProvided_Or_GetEndpointNotGenerated()
    {
        // Arrange
        var sut = new CreateCommandCrudGenerator(_crudGeneratorScheme, null, null);

        // Act
        sut.RunGenerator();

        // Assert
        var endpoint = sut.GeneratedFiles.Find(x => x.FileName.Equals("CreateTestEntityEndpoint.g.cs"));
        endpoint.Should().NotBeNull();
        endpoint!.Source.ToString().Should().Contain(@"var resourceRoute = $""""");
        endpoint.Source.ToString().Should().Contain("return TypedResults.Created(resourceRoute, result);");
    }
}