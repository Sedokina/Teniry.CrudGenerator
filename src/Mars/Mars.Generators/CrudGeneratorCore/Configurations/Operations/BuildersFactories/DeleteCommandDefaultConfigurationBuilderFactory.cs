using Mars.Generators.ApplicationGenerators.Configurations.Global;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders.TypedBuilders;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.BuildersFactories;

internal class DeleteCommandDefaultConfigurationBuilderFactory
{
    public static CqrsOperationWithoutReturnValueConfigurationBuilder Construct(
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration)
    {
        return new CqrsOperationWithoutReturnValueConfigurationBuilder
        {
            GlobalConfiguration = globalConfiguration,
            OperationsSharedConfiguration = operationsSharedConfiguration,
            OperationType = CqrsOperationType.Command,
            OperationName = new NameConfigurationBuilder("Delete{{entity_name}}"),
            Operation = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Delete.DeleteCommand.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("Delete{{entity_name}}Command")
            },
            Handler = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Delete.DeleteHandler.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("Delete{{entity_name}}Handler")
            },
            Endpoint = new MinimalApiEndpointConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Delete.DeleteEndpoint.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("Delete{{entity_name}}Endpoint"),
                FunctionName = new("DeleteAsync"),
                RouteConfigurationBuilder =
                    new EndpointRouteConfigurationBuilder("/{{entity_name}}/{{id_param_name}}/delete")
            }
        };
    }
}