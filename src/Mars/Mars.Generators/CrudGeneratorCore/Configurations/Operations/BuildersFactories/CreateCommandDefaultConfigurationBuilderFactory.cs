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
        // TODO: create operation name and move Create into it, than use like {{ }}
        return new CqrsOperationWithReturnValueConfigurationBuilder
        {
            GlobalConfiguration = globalConfiguration,
            OperationsSharedConfiguration = operationsSharedConfiguration,
            OperationType = CqrsOperationType.Command,
            OperationGroup = new NameConfigurationBuilder("Create{{entity_name}}"),
            Operation = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.Create.CreateCommand.txt"),
                NameConfigurationBuilder = new NameConfigurationBuilder("Create{{entity_name}}Command")
            },
            Dto = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.Create.CreatedDto.txt"),
                NameConfigurationBuilder = new NameConfigurationBuilder("Created{{entity_name}}Dto")
            },
            Handler = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.Create.CreateHandler.txt"),
                NameConfigurationBuilder = new NameConfigurationBuilder("Create{{entity_name}}Handler")
            },
            Endpoint = new MinimalApiEndpointConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.Create.CreateEndpoint.txt"),
                NameConfigurationBuilder = new NameConfigurationBuilder("Create{{entity_name}}Endpoint"),
                FunctionName = new("CreateAsync"),
                RouteConfigurationBuilder = new EndpointRouteConfigurationBuilder("/{{entity_name}}/create")
            }
        };
    }
}