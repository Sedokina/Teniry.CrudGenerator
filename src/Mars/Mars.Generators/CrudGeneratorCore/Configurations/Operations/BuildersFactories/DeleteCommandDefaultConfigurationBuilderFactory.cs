using Mars.Generators.CrudGeneratorCore.Configurations.Global;
using Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders;
using Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;

namespace Mars.Generators.CrudGeneratorCore.Configurations.Operations.BuildersFactories;

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
            OperationGroup = new NameConfigurationBuilder("Delete{{entity_name}}"),
            Operation = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.Delete.DeleteCommand.txt"),
                NameConfigurationBuilder = new NameConfigurationBuilder("Delete{{entity_name}}Command")
            },
            Handler = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.Delete.DeleteHandler.txt"),
                NameConfigurationBuilder = new NameConfigurationBuilder("Delete{{entity_name}}Handler")
            },
            Endpoint = new MinimalApiEndpointConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.Delete.DeleteEndpoint.txt"),
                NameConfigurationBuilder = new NameConfigurationBuilder("Delete{{entity_name}}Endpoint"),
                FunctionName = new("DeleteAsync"),
                RouteConfigurationBuilder =
                    new EndpointRouteConfigurationBuilder("/{{entity_name}}/{{id_param_name}}/delete")
            }
        };
    }
}