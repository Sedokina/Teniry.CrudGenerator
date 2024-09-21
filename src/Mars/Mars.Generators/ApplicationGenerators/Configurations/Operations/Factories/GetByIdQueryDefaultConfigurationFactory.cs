using Mars.Generators.ApplicationGenerators.Configurations.Global;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.TypedConfigurations;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.Factories;

public class GetByIdQueryDefaultConfigurationFactory
{
    public static CqrsOperationWithReturnValueGeneratorConfiguration Construct(
        GlobalCqrsGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfiguration operationsSharedConfiguration)
    {
        return new CqrsOperationWithReturnValueGeneratorConfiguration
        {
            GlobalConfiguration = globalConfiguration,
            OperationsSharedConfiguration = operationsSharedConfiguration,
            OperationType = CqrsOperationType.Query,
            FunctionName = new NameConfiguration("Get{{entity_name}}"),
            Operation = new FileTemplateBasedOperationConfiguration
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.GetById.GetByIdQuery.txt",
                NameConfiguration = new NameConfiguration("Get{{entity_name}}Query")
            },
            Dto = new FileTemplateBasedOperationConfiguration
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.GetById.GetByIdDto.txt",
                NameConfiguration = new NameConfiguration("{{entity_name}}Dto")
            },
            Handler = new FileTemplateBasedOperationConfiguration
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.GetById.GetByIdHandler.txt",
                NameConfiguration = new NameConfiguration("Get{{entity_name}}Handler")
            },
            Endpoint = new MinimalApiEndpointConfiguration
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.GetById.GetByIdEndpoint.txt",
                NameConfiguration = new NameConfiguration("Get{{entity_name}}Endpoint"),
                RouteConfiguration = new EndpointRouteConfiguration("/{{entity_name}}/{{id_param_name}}", "GetAsync")
            }
        };
    }
}