using System.Collections.Generic;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.DbContext;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator.Operations;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;

internal class DeleteCommandDefaultConfigurationBuilderFactory : IConfigurationBuilderFactory
{
    private CqrsOperationWithoutReturnValueConfigurationBuilder _builder;
    private EntityScheme _entityScheme;
    private DbContextScheme _dbContextScheme;

    public CqrsOperationWithoutReturnValueConfigurationBuilder Construct(
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        InternalEntityGeneratorDeleteOperationConfiguration? operationConfiguration)
    {
        return new CqrsOperationWithoutReturnValueConfigurationBuilder
        {
            GlobalConfiguration = globalConfiguration,
            OperationsSharedConfiguration = operationsSharedConfiguration,
            Generate = operationConfiguration?.Generate ?? true,
            OperationType = CqrsOperationType.Command,
            OperationName = operationConfiguration?.Operation ?? "Delete",
            OperationGroup = new(operationConfiguration?.OperationGroup ?? "{{operation_name}}{{entity_name}}"),
            Operation = new(operationConfiguration?.CommandName ?? "{{operation_name}}{{entity_name}}Command"),
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
                                                "/{{entity_name}}/{{id_param_name}}/{{operation_name | string.downcase}}")
            }
        };
    }

    public List<GeneratorResult> RunGenerator(List<EndpointMap> endpointsMaps)
    {
        var deleteCommandConfiguration = _builder.Build(_entityScheme);
        if (!deleteCommandConfiguration.Generate) return [];
        var deleteCommandScheme =
            new CrudGeneratorScheme<CqrsOperationWithoutReturnValueGeneratorConfiguration>(
                _entityScheme,
                _dbContextScheme,
                deleteCommandConfiguration);
        var generateDeleteCommand = new DeleteCommandCrudGenerator(deleteCommandScheme);
        generateDeleteCommand.RunGenerator();
        if (generateDeleteCommand.EndpointMap is not null)
        {
            endpointsMaps.Add(generateDeleteCommand.EndpointMap);
        }

        return generateDeleteCommand.GeneratedFiles;
    }
}