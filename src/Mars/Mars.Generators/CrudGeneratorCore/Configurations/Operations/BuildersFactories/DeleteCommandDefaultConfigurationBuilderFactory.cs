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
            OperationName = "Delete",
            OperationGroup = new NameConfigurationBuilder("{{operation_name}}{{entity_name}}"),
            Operation = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.{{operation_name}}.{{operation_name}}Command.txt"),
                NameConfigurationBuilder = new NameConfigurationBuilder("{{operation_name}}{{entity_name}}Command")
            },
            Handler = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.{{operation_name}}.{{operation_name}}Handler.txt"),
                NameConfigurationBuilder = new NameConfigurationBuilder("{{operation_name}}{{entity_name}}Handler")
            },
            Endpoint = new MinimalApiEndpointConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.{{operation_name}}.{{operation_name}}Endpoint.txt"),
                NameConfigurationBuilder = new NameConfigurationBuilder("{{operation_name}}{{entity_name}}Endpoint"),
                FunctionName = new("{{operation_name}}Async"),
                RouteConfigurationBuilder =
                    new EndpointRouteConfigurationBuilder("/{{entity_name}}/{{id_param_name}}/{{operation_name | string.downcase}}")
            }
        };
    }
}