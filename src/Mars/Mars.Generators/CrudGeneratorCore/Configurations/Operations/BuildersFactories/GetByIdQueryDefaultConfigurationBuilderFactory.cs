using Mars.Generators.CrudGeneratorCore.Configurations.Global;
using Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders;
using Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;

namespace Mars.Generators.CrudGeneratorCore.Configurations.Operations.BuildersFactories;

internal class GetByIdQueryDefaultConfigurationBuilderFactory
{
    public static CqrsOperationWithReturnValueConfigurationBuilder Construct(
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration)
    {
        return new CqrsOperationWithReturnValueConfigurationBuilder
        {
            GlobalConfiguration = globalConfiguration,
            OperationsSharedConfiguration = operationsSharedConfiguration,
            OperationName = "Get",
            OperationType = CqrsOperationType.Query,
            OperationGroup = new NameConfigurationBuilder("{{operation_name}}{{entity_name}}"),
            Operation = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.GetById.GetByIdQuery.txt"),
                NameConfigurationBuilder = new NameConfigurationBuilder("{{operation_name}}{{entity_name}}Query")
            },
            Dto = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.GetById.GetByIdDto.txt"),
                NameConfigurationBuilder = new NameConfigurationBuilder("{{entity_name}}Dto")
            },
            Handler = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.GetById.GetByIdHandler.txt"),
                NameConfigurationBuilder = new NameConfigurationBuilder("{{operation_name}}{{entity_name}}Handler")
            },
            Endpoint = new MinimalApiEndpointConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.GetById.GetByIdEndpoint.txt"),
                NameConfigurationBuilder = new NameConfigurationBuilder("{{operation_name}}{{entity_name}}Endpoint"),
                FunctionName = new("{{operation_name}}Async"),
                RouteConfigurationBuilder = new EndpointRouteConfigurationBuilder("/{{entity_name}}/{{id_param_name}}")
            }
        };
    }
}