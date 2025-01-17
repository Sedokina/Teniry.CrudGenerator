using System.Collections.Generic;
using Teniry.CrudGenerator.Core.Configurations.Configurators;
using Teniry.CrudGenerator.Core.Configurations.Crud;
using Teniry.CrudGenerator.Core.Configurations.Crud.TypedConfigurations;
using Teniry.CrudGenerator.Core.Configurations.Global;
using Teniry.CrudGenerator.Core.Configurations.Shared;
using Teniry.CrudGenerator.Core.Generators;
using Teniry.CrudGenerator.Core.Generators.Core;
using Teniry.CrudGenerator.Core.Schemes.DbContext;
using Teniry.CrudGenerator.Core.Schemes.Entity;
using Teniry.CrudGenerator.Core.Schemes.InternalEntityGenerator.Operations;

namespace Teniry.CrudGenerator.Core.Runners;

internal record CreateCommandGeneratorRunner : IGeneratorRunner {
    private readonly DbContextScheme _dbContextScheme;
    private readonly EntityScheme _entityScheme;
    private readonly EndpointRouteConfigurator? _getByIdEndpointRouteConfigurationBuilder;
    private readonly string _getByIdOperationName;
    public CqrsOperationWithReturnValueGeneratorConfiguration Configuration { get; }

    public CreateCommandGeneratorRunner(
        GlobalCrudGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurator operationsSharedConfiguration,
        InternalEntityGeneratorCreateOperationConfiguration? operationConfiguration,
        EntityScheme entityScheme,
        DbContextScheme dbContextScheme,
        EndpointRouteConfigurator? getByIdEndpointRouteConfigurationBuilder,
        string getByIdOperationName
    ) {
        Configuration = ConstructBuilder(
            globalConfiguration,
            operationsSharedConfiguration,
            operationConfiguration,
            entityScheme
        );
        _entityScheme = entityScheme;
        _dbContextScheme = dbContextScheme;
        _getByIdEndpointRouteConfigurationBuilder = getByIdEndpointRouteConfigurationBuilder;
        _getByIdOperationName = getByIdOperationName;
    }

    public List<GeneratorResult> RunGenerator(List<EndpointMap> endpointsMaps) {
        if (!Configuration.Generate) return [];

        var createCommandScheme =
            new CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration>(
                _entityScheme,
                _dbContextScheme,
                Configuration
            );
        var generateCreateCommand = new CreateCommandCrudGenerator(
            createCommandScheme,
            _getByIdEndpointRouteConfigurationBuilder,
            _getByIdOperationName
        );
        generateCreateCommand.RunGenerator();
        if (generateCreateCommand.EndpointMap is not null) {
            endpointsMaps.Add(generateCreateCommand.EndpointMap);
        }

        return generateCreateCommand.GeneratedFiles;
    }

    private static CqrsOperationWithReturnValueGeneratorConfiguration ConstructBuilder(
        GlobalCrudGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurator operationsSharedConfiguration,
        InternalEntityGeneratorCreateOperationConfiguration? operationConfiguration,
        EntityScheme entityScheme
    ) {
        return new(
            operationConfiguration?.Generate ?? true,
            globalConfiguration,
            operationsSharedConfiguration,
            CqrsOperationType.Command,
            operationConfiguration?.Operation ?? "Create",
            new(operationConfiguration?.OperationGroup ?? "{{operation_name}}{{entity_name}}"),
            new(operationConfiguration?.CommandName ?? "{{operation_name}}{{entity_name}}Command"),
            new(operationConfiguration?.DtoName ?? "Created{{entity_name}}Dto"),
            new(operationConfiguration?.HandlerName ?? "{{operation_name}}{{entity_name}}Handler"),
            new() {
                // If general generate is false, than endpoint generate is also false
                Generate = operationConfiguration?.Generate != false &&
                    (operationConfiguration?.GenerateEndpoint ?? true),
                ClassName = new(
                    operationConfiguration?.EndpointClassName ??
                    "{{operation_name}}{{entity_name}}Endpoint"
                ),
                FunctionName = new(operationConfiguration?.EndpointFunctionName ?? "{{operation_name}}Async"),
                RouteConfigurator = new(
                    operationConfiguration?.RouteName ??
                    "/{{entity_name}}/{{operation_name | string.downcase}}"
                )
            },
            entityScheme
        );
    }
}