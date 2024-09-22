using Mars.Generators.CrudGeneratorCore.Configurations.Global;
using Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders;
using Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;

namespace Mars.Generators.CrudGeneratorCore.Configurations.Operations.BuildersFactories;

internal class CreateCommandDefaultConfigurationBuilderFactory
{
    public static CqrsOperationWithReturnValueConfigurationBuilder Construct(
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration)
    {
        return new CqrsOperationWithReturnValueConfigurationBuilder
        {
            GlobalConfiguration = globalConfiguration,
            OperationsSharedConfiguration = operationsSharedConfiguration,
            OperationType = CqrsOperationType.Command,
            OperationName = "Create",
            OperationGroup = new NameConfigurationBuilder("{{operation_name}}{{entity_name}}"),
            Operation = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.{{operation_name}}.{{operation_name}}Command.txt"),
                NameConfigurationBuilder = new NameConfigurationBuilder("{{operation_name}}{{entity_name}}Command")
            },
            Dto = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.{{operation_name}}.CreatedDto.txt"),
                NameConfigurationBuilder = new NameConfigurationBuilder("Created{{entity_name}}Dto")
            },
            Handler = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.{{operation_name}}.{{operation_name}}Handler.txt"),
                NameConfigurationBuilder = new NameConfigurationBuilder("{{operation_name}}{{entity_name}}Handler")
            },
            Endpoint = new MinimalApiEndpointConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.{{operation_name}}.{{operation_name}}Endpoint.txt"),
                NameConfigurationBuilder = new("{{operation_name}}{{entity_name}}Endpoint"),
                FunctionName = new("{{operation_name}}Async"),
                RouteConfigurationBuilder = new("/{{entity_name}}/{{operation_name | string.downcase}}")
            }
        };
    }
}