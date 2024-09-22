using Mars.Generators.ApplicationGenerators.Configurations.Global;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders.TypedBuilders;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.BuildersFactories;

public class GetByIdQueryDefaultConfigurationBuilderFactory
{
    public static CqrsOperationWithReturnValueConfigurationBuilder Construct(
        GlobalCqrsGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfiguration operationsSharedConfiguration)
    {
        return new CqrsOperationWithReturnValueConfigurationBuilder
        {
            GlobalConfiguration = globalConfiguration,
            OperationsSharedConfiguration = operationsSharedConfiguration,
            OperationType = CqrsOperationType.Query,
            FunctionName = new NameConfigurationBuilder("Get{{entity_name}}"),
            Operation = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.GetById.GetByIdQuery.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("Get{{entity_name}}Query")
            },
            Dto = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.GetById.GetByIdDto.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("{{entity_name}}Dto")
            },
            Handler = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.GetById.GetByIdHandler.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("Get{{entity_name}}Handler")
            },
            Endpoint = new MinimalApiEndpointConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.GetById.GetByIdEndpoint.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("Get{{entity_name}}Endpoint"),
                RouteConfigurationBuilder = new EndpointRouteConfigurationBuilder("/{{entity_name}}/{{id_param_name}}", "GetAsync")
            }
        };
    }
}