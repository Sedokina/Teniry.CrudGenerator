using System.Collections.Generic;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.DbContext;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator.Operations;

namespace ITech.CrudGenerator.CrudGeneratorCore.GeneratorRunners;

internal class UpdateCommandGeneratorRunner : IGeneratorRunner
{
    public CqrsOperationWithReturnValueWithReceiveViewModelGeneratorConfiguration Configuration { get; }
    private readonly EntityScheme _entityScheme;
    private readonly DbContextScheme _dbContextScheme;

    public UpdateCommandGeneratorRunner(
        GlobalCqrsGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        InternalEntityGeneratorUpdateOperationConfiguration? operationConfiguration,
        EntityScheme entityScheme,
        DbContextScheme dbContextScheme)
    {
        Configuration = ConstructConfiguration(
            globalConfiguration,
            operationsSharedConfiguration,
            operationConfiguration,
            entityScheme);
        _entityScheme = entityScheme;
        _dbContextScheme = dbContextScheme;
    }

    private static CqrsOperationWithReturnValueWithReceiveViewModelGeneratorConfiguration ConstructConfiguration(
        GlobalCqrsGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        InternalEntityGeneratorUpdateOperationConfiguration? operationConfiguration,
        EntityScheme entityScheme
    )
    {
        return new CqrsOperationWithReturnValueWithReceiveViewModelGeneratorConfiguration(
            generate: operationConfiguration?.Generate ?? true,
            globalConfiguration: globalConfiguration,
            operationsSharedConfiguration: operationsSharedConfiguration,
            operationType: CqrsOperationType.Command,
            operationName: operationConfiguration?.Operation ?? "Update",
            operationGroup: new(operationConfiguration?.OperationGroup ?? "{{operation_name}}{{entity_name}}"),
            operation: new(operationConfiguration?.CommandName ?? "{{operation_name}}{{entity_name}}Command"),
            handler: new(operationConfiguration?.HandlerName ?? "{{operation_name}}{{entity_name}}Handler"),
            viewModel: new(operationConfiguration?.ViewModelName ?? "{{operation_name}}{{entity_name}}Vm"),
            endpoint: new MinimalApiEndpointConfigurationBuilder
            {
                Generate = operationConfiguration?.Generate != false &&
                           (operationConfiguration?.GenerateEndpoint ?? true),
                ClassName = new(operationConfiguration?.EndpointClassName ??
                                "{{operation_name}}{{entity_name}}Endpoint"),
                FunctionName = new(operationConfiguration?.EndpointFunctionName ?? "{{operation_name}}Async"),
                RouteConfigurationBuilder = new(operationConfiguration?.RouteName ??
                                                "/{{entity_name}}/{{id_param_name}}/{{operation_name | string.downcase}}")
            },
            entityScheme
        );
    }

    public List<GeneratorResult> RunGenerator(List<EndpointMap> endpointsMaps)
    {
        if (!Configuration.Generate) return [];
        var updateCommandScheme =
            new CrudGeneratorScheme<CqrsOperationWithReturnValueWithReceiveViewModelGeneratorConfiguration>(
                _entityScheme,
                _dbContextScheme,
                Configuration);
        var generateUpdateCommand = new UpdateCommandCrudGenerator(updateCommandScheme);
        generateUpdateCommand.RunGenerator();
        if (generateUpdateCommand.EndpointMap is not null)
        {
            endpointsMaps.Add(generateUpdateCommand.EndpointMap);
        }

        return generateUpdateCommand.GeneratedFiles;
    }
}