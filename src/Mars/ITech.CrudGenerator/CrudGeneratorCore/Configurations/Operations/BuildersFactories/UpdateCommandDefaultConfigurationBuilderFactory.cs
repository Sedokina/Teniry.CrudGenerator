using System.Collections.Generic;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.DbContext;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator.Operations;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;

internal class UpdateCommandDefaultConfigurationBuilderFactory : IConfigurationBuilderFactory
{
    private CqrsOperationWithoutReturnValueWithReceiveViewModelConfigurationBuilder _builder;
    private EntityScheme _entityScheme;
    private DbContextScheme _dbContextScheme;

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
            Operation = new(operationConfiguration?.CommandName ?? "{{operation_name}}{{entity_name}}Command"),
            Handler = new(operationConfiguration?.HandlerName ?? "{{operation_name}}{{entity_name}}Handler"),
            ViewModel = new(operationConfiguration?.ViewModelName ?? "{{operation_name}}{{entity_name}}Vm"),
            Endpoint = new()
            {
                // If general generate is false, than endpoint generate is also false
                Generate = operationConfiguration?.Generate != false &&
                           (operationConfiguration?.GenerateEndpoint ?? true),
                ClassName = new(operationConfiguration?.EndpointClassName ??
                                "{{operation_name}}{{entity_name}}Endpoint"),
                FunctionName = new(operationConfiguration?.EndpointFunctionName ?? "{{operation_name}}Async"),
                RouteConfigurationBuilder = new(operationConfiguration?.RouteName ??
                                                "/{{entity_name}}/{{id_param_name}}/{{operation_name | string.downcase}}")
            }
        };
    }

    public List<GeneratorResult> RunGenerator(List<EndpointMap> endpointsMaps)
    {
        var updateOperationConfiguration = _builder.Build(_entityScheme);
        if (!updateOperationConfiguration.Generate) return [];
        var updateCommandScheme =
            new CrudGeneratorScheme<CqrsOperationWithReturnValueWithReceiveViewModelGeneratorConfiguration>(
                _entityScheme,
                _dbContextScheme,
                updateOperationConfiguration);
        var generateUpdateCommand = new UpdateCommandCrudGenerator(updateCommandScheme);
        generateUpdateCommand.RunGenerator();
        if (generateUpdateCommand.EndpointMap is not null)
        {
            endpointsMaps.Add(generateUpdateCommand.EndpointMap);
        }

        return generateUpdateCommand.GeneratedFiles;
    }
}