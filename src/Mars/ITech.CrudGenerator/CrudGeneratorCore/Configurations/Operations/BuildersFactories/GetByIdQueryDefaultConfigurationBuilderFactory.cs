using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;

internal class GetByIdQueryDefaultConfigurationBuilderFactory
{
    public CqrsOperationWithReturnValueConfigurationBuilder Construct(
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        InternalEntityGeneratorGetByIdOperationConfiguration? operationConfiguration)
    {
        return new CqrsOperationWithReturnValueConfigurationBuilder
        {
            GlobalConfiguration = globalConfiguration,
            OperationsSharedConfiguration = operationsSharedConfiguration,
            Generate = operationConfiguration?.Generate ?? true,
            OperationType = CqrsOperationType.Query,
            OperationName = operationConfiguration?.Operation ?? "Get",
            OperationGroup = new(operationConfiguration?.OperationGroup ?? "{{operation_name}}{{entity_name}}"),
            Operation = new()
            {
                TemplatePath = new("{{templates_base_path}}.GetById.GetByIdQuery.txt"),
                NameConfigurationBuilder = new(operationConfiguration?.QueryName ??
                                               "{{operation_name}}{{entity_name}}Query")
            },
            Dto = new()
            {
                TemplatePath = new("{{templates_base_path}}.GetById.GetByIdDto.txt"),
                NameConfigurationBuilder = new(operationConfiguration?.DtoName ?? "{{entity_name}}Dto")
            },
            Handler = new()
            {
                TemplatePath = new("{{templates_base_path}}.GetById.GetByIdHandler.txt"),
                NameConfigurationBuilder = new(operationConfiguration?.HandlerName ??
                                               "{{operation_name}}{{entity_name}}Handler")
            },
            Endpoint = new()
            {
                // If general generate is false, than endpoint generate is also false
                Generate = operationConfiguration?.Generate != false &&
                           (operationConfiguration?.GenerateEndpoint ?? true),
                TemplatePath = new("{{templates_base_path}}.GetById.GetByIdEndpoint.txt"),
                NameConfigurationBuilder = new(operationConfiguration?.EndpointClassName ??
                                               "{{operation_name}}{{entity_name}}Endpoint"),
                FunctionName = new(operationConfiguration?.EndpointFunctionName ?? "{{operation_name}}Async"),
                RouteConfigurationBuilder =
                    new(operationConfiguration?.RouteName ?? "/{{entity_name}}/{{id_param_name}}")
            }
        };
    }
}