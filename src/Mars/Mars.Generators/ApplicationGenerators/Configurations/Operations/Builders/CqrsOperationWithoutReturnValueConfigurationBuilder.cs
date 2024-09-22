using Mars.Generators.ApplicationGenerators.Configurations.Global;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders.TypedBuilders;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.Formatters;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders;

internal class CqrsOperationWithoutReturnValueConfigurationBuilder
{
    public GlobalCqrsGeneratorConfigurationBuilder GlobalConfiguration { get; set; } = null!;
    public CqrsOperationsSharedConfigurationBuilder OperationsSharedConfiguration { get; set; } = null!;
    public CqrsOperationType OperationType { get; set; }
    public NameConfigurationBuilder OperationName { get; set; } = null!;
    public FileTemplateBasedOperationConfigurationBuilder Operation { get; set; } = null!;
    public FileTemplateBasedOperationConfigurationBuilder Handler { get; set; } = null!;
    public MinimalApiEndpointConfigurationBuilder Endpoint { get; set; } = null!;

    protected void Init(CqrsOperationWithoutReturnValueGeneratorConfiguration configuration, EntityScheme entityScheme)
    {
        configuration.GlobalConfiguration = GlobalConfiguration.Build();
        configuration.FunctionName = OperationName.GetName(entityScheme.EntityName);
        configuration.OperationsSharedConfiguration =
            OperationsSharedConfiguration.Build(entityScheme, configuration.FunctionName);
        configuration.OperationType = CqrsOperationType.Command;
        configuration.Operation = new()
        {
            TemplatePath = Operation.TemplatePath,
            Name = Operation.NameConfigurationBuilder.GetName(entityScheme.EntityName),
        };
        configuration.Handler = new()
        {
            TemplatePath = Handler.TemplatePath,
            Name = Handler.NameConfigurationBuilder.GetName(entityScheme.EntityName),
        };

        var constructorParametersForRoute = entityScheme.PrimaryKeys.GetAsMethodCallParameters();
        configuration.Endpoint = new()
        {
            TemplatePath = Endpoint.TemplatePath,
            Name = Endpoint.NameConfigurationBuilder.GetName(entityScheme.EntityName),
            FunctionName = Endpoint.FunctionName.GetName(entityScheme.EntityName),
            Route = Endpoint.RouteConfigurationBuilder
                .GetRoute(entityScheme.EntityName.Name, constructorParametersForRoute)
        };
    }

    public CqrsOperationWithoutReturnValueGeneratorConfiguration Build(EntityScheme entityScheme)
    {
        var result = new CqrsOperationWithoutReturnValueGeneratorConfiguration();
        Init(result, entityScheme);
        return result;
    }
}