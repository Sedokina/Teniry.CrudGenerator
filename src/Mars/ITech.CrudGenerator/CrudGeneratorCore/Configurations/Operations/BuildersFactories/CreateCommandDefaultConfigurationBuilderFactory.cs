using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator.Operations;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;

internal class CreateCommandDefaultConfigurationBuilderFactory
{
    public CqrsOperationWithReturnValueConfigurationBuilder Construct(
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        InternalEntityGeneratorCreateOperationConfiguration? operationConfiguration)
    {
        return new CqrsOperationWithReturnValueConfigurationBuilder
        {
            GlobalConfiguration = globalConfiguration,
            OperationsSharedConfiguration = operationsSharedConfiguration,
            Generate = operationConfiguration?.Generate ?? true,
            OperationType = CqrsOperationType.Command,
            OperationName = operationConfiguration?.Operation ?? "Create",
            OperationGroup = new(operationConfiguration?.OperationGroup ?? "{{operation_name}}{{entity_name}}"),
            Operation = new()
            {
                TemplatePath = new("{{templates_base_path}}.Create.CreateCommand.txt"),
                NameConfigurationBuilder = new(operationConfiguration?.CommandName ??
                                               "{{operation_name}}{{entity_name}}Command")
            },
            Dto = new()
            {
                TemplatePath = new("{{templates_base_path}}.Create.CreatedDto.txt"),
                NameConfigurationBuilder = new(operationConfiguration?.DtoName ??
                                               "Created{{entity_name}}Dto")
            },
            Handler = new()
            {
                TemplatePath = new("{{templates_base_path}}.Create.CreateHandler.txt"),
                NameConfigurationBuilder = new(operationConfiguration?.HandlerName ??
                                               "{{operation_name}}{{entity_name}}Handler")
            },
            Endpoint = new()
            {
                // If general generate is false, than endpoint generate is also false
                Generate = operationConfiguration?.Generate != false &&
                           (operationConfiguration?.GenerateEndpoint ?? true),
                TemplatePath = new("{{templates_base_path}}.Create.CreateEndpoint.txt"),
                NameConfigurationBuilder = new(operationConfiguration?.EndpointClassName ??
                                               "{{operation_name}}{{entity_name}}Endpoint"),
                FunctionName = new(operationConfiguration?.EndpointFunctionName ?? "{{operation_name}}Async"),
                RouteConfigurationBuilder = new(operationConfiguration?.RouteName ??
                                                "/{{entity_name}}/{{operation_name | string.downcase}}")
            }
        };
    }
}