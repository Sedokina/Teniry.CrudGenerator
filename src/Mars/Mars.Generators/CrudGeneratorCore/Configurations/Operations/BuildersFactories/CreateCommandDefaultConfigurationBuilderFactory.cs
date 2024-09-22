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
        // TODO: use TemplatesBasePath not directly, but from {{ }} syntax
        // TODO: create operation name and move Create into it, than use like {{ }}
        // TODO: move function name from route configuration
        return new CqrsOperationWithReturnValueConfigurationBuilder
        {
            GlobalConfiguration = globalConfiguration,
            OperationsSharedConfiguration = operationsSharedConfiguration,
            OperationType = CqrsOperationType.Command,
            OperationName = new NameConfigurationBuilder("Create{{entity_name}}"),
            Operation = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Create.CreateCommand.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("Create{{entity_name}}Command")
            },
            Dto = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Create.CreatedDto.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("Created{{entity_name}}Dto")
            },
            Handler = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Create.CreateHandler.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("Create{{entity_name}}Handler")
            },
            Endpoint = new MinimalApiEndpointConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Create.CreateEndpoint.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("Create{{entity_name}}Endpoint"),
                FunctionName = new("CreateAsync"),
                RouteConfigurationBuilder = new EndpointRouteConfigurationBuilder("/{{entity_name}}/create")
            }
        };
    }
}