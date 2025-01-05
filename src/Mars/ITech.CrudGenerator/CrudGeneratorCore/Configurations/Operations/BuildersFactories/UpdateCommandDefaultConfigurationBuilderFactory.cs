using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator.Operations;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;

internal class UpdateCommandDefaultConfigurationBuilderFactory
{
    public CqrsOperationWithoutReturnValueWithReceiveViewModelConfigurationBuilder Construct(
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        InternalEntityGeneratorUpdateOperationConfiguration? operationConfiguration)
    {
        return new CqrsOperationWithoutReturnValueWithReceiveViewModelConfigurationBuilder
        {
            GlobalConfiguration = globalConfiguration,
            OperationsSharedConfiguration = operationsSharedConfiguration,
            Generate = operationConfiguration?.Generate ?? true,
            OperationType = CqrsOperationType.Command,
            OperationName = operationConfiguration?.Operation ?? "Update",
            OperationGroup = new(operationConfiguration?.OperationGroup ?? "{{operation_name}}{{entity_name}}"),
            Operation = new()
            {
                TemplatePath = new(""),
                NameConfigurationBuilder = new(operationConfiguration?.CommandName ??
                                               "{{operation_name}}{{entity_name}}Command")
            },
            Handler = new()
            {
                TemplatePath = new(""),
                NameConfigurationBuilder = new(operationConfiguration?.HandlerName ??
                                               "{{operation_name}}{{entity_name}}Handler")
            },
            ViewModel = new()
            {
                TemplatePath = new(""),
                NameConfigurationBuilder = new(operationConfiguration?.ViewModelName ??
                                               "{{operation_name}}{{entity_name}}Vm")
            },
            Endpoint = new()
            {
                // If general generate is false, than endpoint generate is also false
                Generate = operationConfiguration?.Generate != false &&
                           (operationConfiguration?.GenerateEndpoint ?? true),
                TemplatePath = new("{{templates_base_path}}.Update.UpdateEndpoint.txt"),
                NameConfigurationBuilder = new(operationConfiguration?.EndpointClassName ??
                                               "{{operation_name}}{{entity_name}}Endpoint"),
                FunctionName = new(operationConfiguration?.EndpointFunctionName ?? "{{operation_name}}Async"),
                RouteConfigurationBuilder = new(operationConfiguration?.RouteName ??
                                                "/{{entity_name}}/{{id_param_name}}/{{operation_name | string.downcase}}")
            }
        };
    }
}