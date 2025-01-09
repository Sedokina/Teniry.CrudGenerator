using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator.Operations;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;

internal class GetByIdQueryDefaultConfigurationBuilderFactory : IConfigurationBuilderFactory
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
            Operation = new(operationConfiguration?.QueryName ?? "{{operation_name}}{{entity_name}}Query"),
            Dto = new(operationConfiguration?.DtoName ?? "{{entity_name}}Dto"),
            Handler = new(operationConfiguration?.HandlerName ?? "{{operation_name}}{{entity_name}}Handler"),
            Endpoint = new()
            {
                // If general generate is false, than endpoint generate is also false
                Generate = operationConfiguration?.Generate != false &&
                           (operationConfiguration?.GenerateEndpoint ?? true),
                ClassName = new(operationConfiguration?.EndpointClassName ??
                                "{{operation_name}}{{entity_name}}Endpoint"),
                FunctionName = new(operationConfiguration?.EndpointFunctionName ?? "{{operation_name}}Async"),
                RouteConfigurationBuilder =
                    new(operationConfiguration?.RouteName ?? "/{{entity_name}}/{{id_param_name}}")
            }
        };
    }
}