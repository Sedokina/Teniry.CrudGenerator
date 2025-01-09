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

internal class GetByIdQueryGeneratorRunner : IGeneratorRunner
{
    public CqrsOperationWithReturnValueGeneratorConfiguration Configuration { get; }
    private readonly EntityScheme _entityScheme;
    private readonly DbContextScheme _dbContextScheme;

    public GetByIdQueryGeneratorRunner(
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        InternalEntityGeneratorGetByIdOperationConfiguration? operationConfiguration,
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

    private static CqrsOperationWithReturnValueGeneratorConfiguration ConstructConfiguration(
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        InternalEntityGeneratorGetByIdOperationConfiguration? operationConfiguration,
        EntityScheme entityScheme)
    {
        return new CqrsOperationWithReturnValueGeneratorConfiguration(
            generate: operationConfiguration?.Generate ?? true,
            globalConfiguration: globalConfiguration,
            operationsSharedConfiguration: operationsSharedConfiguration,
            operationType: CqrsOperationType.Query,
            operationName: operationConfiguration?.Operation ?? "Get",
            operationGroup: new(operationConfiguration?.OperationGroup ?? "{{operation_name}}{{entity_name}}"),
            operation: new(operationConfiguration?.QueryName ?? "{{operation_name}}{{entity_name}}Query"),
            dto: new(operationConfiguration?.DtoName ?? "{{entity_name}}Dto"),
            handler: new(operationConfiguration?.HandlerName ?? "{{operation_name}}{{entity_name}}Handler"),
            endpoint: new MinimalApiEndpointConfigurationBuilder
            {
                Generate = operationConfiguration?.Generate != false &&
                           (operationConfiguration?.GenerateEndpoint ?? true),
                ClassName = new(operationConfiguration?.EndpointClassName ??
                                "{{operation_name}}{{entity_name}}Endpoint"),
                FunctionName = new(operationConfiguration?.EndpointFunctionName ?? "{{operation_name}}Async"),
                RouteConfigurationBuilder = GetRouteConfigurationBuilder(operationConfiguration)
            },
            entityScheme
        );
    }

    public static EndpointRouteConfigurationBuilder GetRouteConfigurationBuilder(
        InternalEntityGeneratorGetByIdOperationConfiguration? operationConfiguration)
    {
        return new(operationConfiguration?.RouteName ?? "/{{entity_name}}/{{id_param_name}}");
    }

    public List<GeneratorResult> RunGenerator(List<EndpointMap> endpointsMaps)
    {
        if (!Configuration.Generate) return [];
        var getByIdQueryScheme = new CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration>(
            _entityScheme,
            _dbContextScheme,
            Configuration
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