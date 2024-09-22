using Mars.Generators.ApplicationGenerators.Configurations.Global;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders.TypedBuilders;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders;

public class CqrsOperationWithoutReturnValueGeneratorConfigurationBuilder
{
    public GlobalCqrsGeneratorConfiguration GlobalConfiguration { get; set; }
    public CqrsOperationsSharedConfiguration OperationsSharedConfiguration { get; set; }
    public CqrsOperationType OperationType { get; set; }
    public NameConfigurationBuilder FunctionName { get; set; }
    public FileTemplateBasedOperationConfigurationBuilder Operation { get; set; }
    public FileTemplateBasedOperationConfigurationBuilder Handler { get; set; }
    public MinimalApiEndpointConfigurationBuilder Endpoint { get; set; }

    protected void Init(CqrsOperationWithoutReturnValueGeneratorConfiguration configuration, EntityName entityName)
    {
        configuration.GlobalConfiguration = GlobalConfiguration;
        configuration.OperationsSharedConfiguration = OperationsSharedConfiguration;
        configuration.OperationType = CqrsOperationType.Command;
        configuration.FunctionName = FunctionName.GetName(entityName);
        configuration.Operation = new()
        {
            TemplatePath = Operation.TemplatePath,
            Name = Operation.NameConfigurationBuilder.GetName(entityName),
        };
        configuration.Handler = new()
        {
            TemplatePath = Handler.TemplatePath,
            Name = Handler.NameConfigurationBuilder.GetName(entityName),
        };
        configuration.Endpoint = new()
        {
            TemplatePath = Endpoint.TemplatePath,
            Name = Endpoint.NameConfigurationBuilder.GetName(entityName),
            RouteConfigurationBuilder = Endpoint.RouteConfigurationBuilder
        };
    }

    public CqrsOperationWithoutReturnValueGeneratorConfiguration Build(EntityName entityName)
    {
        var result = new CqrsOperationWithoutReturnValueGeneratorConfiguration();
        Init(result, entityName);
        return result;
    }
}