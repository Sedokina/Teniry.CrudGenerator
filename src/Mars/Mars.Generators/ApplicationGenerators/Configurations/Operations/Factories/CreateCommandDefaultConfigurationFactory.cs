using Mars.Generators.ApplicationGenerators.Configurations.Global;
using Mars.Generators.ApplicationGenerators.Configurations.Global.TypedConfigurations;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.TypedConfigurations;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.Factories;

public class CreateCommandDefaultConfigurationFactory
{
    public static CqrsOperationWithReturnValueGeneratorConfiguration Construct(
        GlobalCqrsGeneratorConfiguration globalConfiguration)
    {
        // TODO: use TemplatesBasePath not directly, but from {{ }} syntax
        // TODO: create operation name and move Create into it, than use like {{ }}
        // TODO: move function name from route configuration
        return new CqrsOperationWithReturnValueGeneratorConfiguration
        {
            GlobalConfiguration = globalConfiguration,
            OperationType = CqrsOperationType.Command,
            FunctionName = new NameConfiguration("Create{{entity_name}}"),
            Operation = new FileTemplateBasedOperationConfiguration
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Create.CreateCommand.txt",
                NameConfiguration = new NameConfiguration("Create{{entity_name}}Command")
            },
            Dto = new FileTemplateBasedOperationConfiguration
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Create.CreatedDto.txt",
                NameConfiguration = new NameConfiguration("Created{{entity_name}}Dto")
            },
            Handler = new FileTemplateBasedOperationConfiguration
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Create.CreateHandler.txt",
                NameConfiguration = new NameConfiguration("Create{{entity_name}}Handler")
            },
            Endpoint = new MinimalApiEndpointConfiguration
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Create.CreateEndpoint.txt",
                NameConfiguration = new NameConfiguration("Create{{entity_name}}Endpoint"),
                RouteConfiguration = new EndpointRouteConfiguration("/{{entity_name}}/create", "CreateAsync")
            }
        };
    }
}