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

internal class CreateCommandGeneratorRunner : IGeneratorRunner
{
    public CqrsOperationWithReturnValueGeneratorConfiguration Configuration { get; }
    private readonly EntityScheme _entityScheme;
    private readonly DbContextScheme _dbContextScheme;
    private readonly EndpointRouteConfigurationBuilder? _getByIdEndpointRouteConfigurationBuilder;
    private readonly string _getByIdOperationName;

    public CreateCommandGeneratorRunner(
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        InternalEntityGeneratorCreateOperationConfiguration? operationConfiguration,
        EntityScheme entityScheme,
        DbContextScheme dbContextScheme,
        EndpointRouteConfigurationBuilder? getByIdEndpointRouteConfigurationBuilder,
        string getByIdOperationName)
    {
        Configuration = ConstructBuilder(
            globalConfiguration,
            operationsSharedConfiguration,
            operationConfiguration,
            entityScheme);
        _entityScheme = entityScheme;
        _dbContextScheme = dbContextScheme;
        _getByIdEndpointRouteConfigurationBuilder = getByIdEndpointRouteConfigurationBuilder;
        _getByIdOperationName = getByIdOperationName;
    }

    private static CqrsOperationWithReturnValueGeneratorConfiguration ConstructBuilder(
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        InternalEntityGeneratorCreateOperationConfiguration? operationConfiguration,
        EntityScheme entityScheme)
    {
        return new CqrsOperationWithReturnValueGeneratorConfiguration(
            generate: operationConfiguration?.Generate ?? true,
            globalConfiguration: globalConfiguration,
            operationsSharedConfiguration: operationsSharedConfiguration,
            operationType: CqrsOperationType.Command,
            operationName: operationConfiguration?.Operation ?? "Create",
            operationGroup: new(operationConfiguration?.OperationGroup ?? "{{operation_name}}{{entity_name}}"),
            operation: new(operationConfiguration?.CommandName ?? "{{operation_name}}{{entity_name}}Command"),
            dto: new(operationConfiguration?.DtoName ?? "Created{{entity_name}}Dto"),
            handler: new(operationConfiguration?.HandlerName ?? "{{operation_name}}{{entity_name}}Handler"),
            endpoint: new MinimalApiEndpointConfigurationBuilder
            {
                // If general generate is false, than endpoint generate is also false
                Generate = operationConfiguration?.Generate != false &&
                           (operationConfiguration?.GenerateEndpoint ?? true),
                ClassName = new(operationConfiguration?.EndpointClassName ??
                                "{{operation_name}}{{entity_name}}Endpoint"),
                FunctionName = new(operationConfiguration?.EndpointFunctionName ?? "{{operation_name}}Async"),
                RouteConfigurationBuilder = new(operationConfiguration?.RouteName ??
                                                "/{{entity_name}}/{{operation_name | string.downcase}}")
            },
            entityScheme
        );
    }

    public List<GeneratorResult> RunGenerator(List<EndpointMap> endpointsMaps)
    {
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
        if (generateCreateCommand.EndpointMap is not null)
        {
            endpointsMaps.Add(generateCreateCommand.EndpointMap);
        }

        return generateCreateCommand.GeneratedFiles;
    }
}