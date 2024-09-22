using Mars.Generators.CrudGeneratorCore.Configurations.Global;
using Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders;
using Mars.Generators.CrudGeneratorCore.Schemes.EntityCustomization;

namespace Mars.Generators.CrudGeneratorCore.Configurations.Operations.BuildersFactories;

internal class CreateCommandDefaultConfigurationBuilderFactory
{
    public static CqrsOperationWithReturnValueConfigurationBuilder Construct(
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        EntityCreateOperationCustomizationScheme customizationScheme)
    {
        return new CqrsOperationWithReturnValueConfigurationBuilder
        {
            GlobalConfiguration = globalConfiguration,
            OperationsSharedConfiguration = operationsSharedConfiguration,
            Generate = customizationScheme.Generate ?? true,
            OperationType = CqrsOperationType.Command,
            OperationName = customizationScheme.OperationType ?? "Create",
            OperationGroup = new(customizationScheme.OperationGroup ?? "{{operation_name}}{{entity_name}}"),
            Operation = new()
            {
                TemplatePath = new("{{templates_base_path}}.Create.CreateCommand.txt"),
                NameConfigurationBuilder = new(customizationScheme.OperationName ??
                                               "{{operation_name}}{{entity_name}}Command")
            },
            Dto = new()
            {
                TemplatePath = new("{{templates_base_path}}.Create.CreatedDto.txt"),
                NameConfigurationBuilder = new(customizationScheme.DtoName ??
                                               "Created{{entity_name}}Dto")
            },
            Handler = new()
            {
                TemplatePath = new("{{templates_base_path}}.Create.CreateHandler.txt"),
                NameConfigurationBuilder = new(customizationScheme.HandlerName ??
                                               "{{operation_name}}{{entity_name}}Handler")
            },
            Endpoint = new()
            {
                TemplatePath = new("{{templates_base_path}}.Create.CreateEndpoint.txt"),
                NameConfigurationBuilder = new(customizationScheme.EndpointClassName ??
                                               "{{operation_name}}{{entity_name}}Endpoint"),
                FunctionName = new(customizationScheme.EndpointFunctionName ?? "{{operation_name}}Async"),
                RouteConfigurationBuilder = new(customizationScheme.RouteName ??
                                                "/{{entity_name}}/{{operation_name | string.downcase}}")
            }
        };
    }
}