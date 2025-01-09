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

internal class GetByIdQueryDefaultConfigurationBuilderFactory : IConfigurationBuilderFactory
{
    public CqrsOperationWithReturnValueConfigurationBuilder Builder { get; }
    private readonly EntityScheme _entityScheme;
    private readonly DbContextScheme _dbContextScheme;

    public GetByIdQueryDefaultConfigurationBuilderFactory(
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        InternalEntityGeneratorGetByIdOperationConfiguration? operationConfiguration,
        EntityScheme entityScheme,
        DbContextScheme dbContextScheme)
    {
        Builder = ConstructBuilder(globalConfiguration, operationsSharedConfiguration, operationConfiguration);
        _entityScheme = entityScheme;
        _dbContextScheme = dbContextScheme;
    }

    private static CqrsOperationWithReturnValueConfigurationBuilder ConstructBuilder(
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

    public List<GeneratorResult> RunGenerator(List<EndpointMap> endpointsMaps)
    {
        var getByIdQueryConfiguration = Builder.Build(_entityScheme);
        if (!getByIdQueryConfiguration.Generate) return [];
        var getByIdQueryScheme = new CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration>(
            _entityScheme,
            _dbContextScheme,
            getByIdQueryConfiguration
        );
        var generateGetByIdQuery = new GetByIdQueryCrudGenerator(getByIdQueryScheme);
        generateGetByIdQuery.RunGenerator();
        if (generateGetByIdQuery.EndpointMap is not null)
        {
            endpointsMaps.Add(generateGetByIdQuery.EndpointMap);
        }

        return generateGetByIdQuery.GeneratedFiles;
    }
}