using Mars.Generators.ApplicationGenerators.Configurations.Global;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders.TypedBuilders;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.Factories;

public class GetListQueryDefaultConfigurationFactory
{
    public static CqrsListOperationWithoutReturnValueGeneratorConfigurationBuilder Construct(
        GlobalCqrsGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfiguration operationsSharedConfiguration)
    {
        return new CqrsListOperationWithoutReturnValueGeneratorConfigurationBuilder
        {
            GlobalConfiguration = globalConfiguration,
            OperationsSharedConfiguration = operationsSharedConfiguration,
            OperationType = CqrsOperationType.Query,
            FunctionName = new NameConfigurationBuilder("GetList{{entity_name}}"),
            Operation = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.GetList.GetListQuery.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("Get{{plural_entity_name}}Query")
            },
            Dto = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.GetList.GetListDto.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("{{plural_entity_name}}Dto")
            },
            DtoListItem = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.GetList.GetListItemDto.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("{{plural_entity_name}}ListItemDto")
            },
            Filter = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.GetList.GetListFilter.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("Get{{plural_entity_name}}Filter")
            },
            Handler = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.GetList.GetListHandler.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("Get{{plural_entity_name}}Handler")
            },
            Endpoint = new MinimalApiEndpointConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.GetList.GetListEndpoint.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("Get{{plural_entity_name}}Endpoint"),
                RouteConfigurationBuilder = new EndpointRouteConfigurationBuilder("/{{entity_name}}", "GetAsync")
            }
        };
    }
}