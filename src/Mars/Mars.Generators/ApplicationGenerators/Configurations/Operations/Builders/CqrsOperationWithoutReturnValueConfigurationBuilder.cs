using Mars.Generators.ApplicationGenerators.Configurations.Global;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders.TypedBuilders;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.Formatters;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders;

internal class CqrsOperationWithoutReturnValueConfigurationBuilder
{
    public GlobalCqrsGeneratorConfiguration GlobalConfiguration { get; set; }
    public CqrsOperationsSharedConfiguration OperationsSharedConfiguration { get; set; }
    public CqrsOperationType OperationType { get; set; }
    public NameConfigurationBuilder FunctionName { get; set; }
    public FileTemplateBasedOperationConfigurationBuilder Operation { get; set; }
    public FileTemplateBasedOperationConfigurationBuilder Handler { get; set; }
    public MinimalApiEndpointConfigurationBuilder Endpoint { get; set; }

    protected void Init(CqrsOperationWithoutReturnValueGeneratorConfiguration configuration, EntityScheme entityScheme)
    {
        configuration.GlobalConfiguration = GlobalConfiguration;
        configuration.OperationsSharedConfiguration = OperationsSharedConfiguration;
        configuration.OperationType = CqrsOperationType.Command;
        configuration.FunctionName = FunctionName.GetName(entityScheme.EntityName);
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