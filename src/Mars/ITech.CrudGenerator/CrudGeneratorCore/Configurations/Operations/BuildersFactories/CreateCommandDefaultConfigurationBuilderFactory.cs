using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator.Operations;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;

internal class CreateCommandDefaultConfigurationBuilderFactory : IConfigurationBuilderFactory
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
            Operation = new(operationConfiguration?.CommandName ?? "{{operation_name}}{{entity_name}}Command"),
            Dto = new(operationConfiguration?.DtoName ?? "Created{{entity_name}}Dto"),
            Handler = new(operationConfiguration?.HandlerName ?? "{{operation_name}}{{entity_name}}Handler"),
            Endpoint = new()
            {
                // If general generate is false, than endpoint generate is also false
                Generate = operationConfiguration?.Generate != false &&
                           (operationConfiguration?.GenerateEndpoint ?? true),
                ClassName = new(operationConfiguration?.EndpointClassName ??
                                "{{operation_name}}{{entity_name}}Endpoint"),
                FunctionName = new(operationConfiguration?.EndpointFunctionName ?? "{{operation_name}}Async"),
                RouteConfigurationBuilder = new(operationConfiguration?.RouteName ??
                                                "/{{entity_name}}/{{operation_name | string.downcase}}")
            }
        };
    }

    public object Construct(GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        InternalEntityGeneratorConfiguration internalEntityGeneratorConfiguration)
    {
        return Construct(globalConfiguration,
            operationsSharedConfiguration,
            internalEntityGeneratorConfiguration.CreateOperation
        );
    }
}