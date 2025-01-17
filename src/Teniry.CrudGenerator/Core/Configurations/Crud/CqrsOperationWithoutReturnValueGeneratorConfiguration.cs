using Teniry.CrudGenerator.Core.Schemes.Entity.Formatters;
using Teniry.CrudGenerator.Core.Configurations.Configurators;
using Teniry.CrudGenerator.Core.Configurations.Crud.TypedConfigurations;
using Teniry.CrudGenerator.Core.Configurations.Global;
using Teniry.CrudGenerator.Core.Configurations.Shared;
using Teniry.CrudGenerator.Core.Schemes.Entity;

namespace Teniry.CrudGenerator.Core.Configurations.Crud;

internal record CqrsOperationWithoutReturnValueGeneratorConfiguration {
    public bool Generate { get; }
    public GlobalCrudGeneratorConfiguration GlobalConfiguration { get; }
    public CqrsOperationsSharedConfiguration OperationsSharedConfiguration { get; }
    public CqrsOperationType OperationType { get; set; }
    public string OperationGroup { get; }
    public string OperationName { get; }
    public string Operation { get; }
    public string Handler { get; }
    public MinimalApiEndpointConfiguration Endpoint { get; }

    public CqrsOperationWithoutReturnValueGeneratorConfiguration(
        bool generate,
        GlobalCrudGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurator operationsSharedConfiguration,
        CqrsOperationType operationType,
        string operationName,
        NameConfigurator operationGroup,
        NameConfigurator operation,
        NameConfigurator handler,
        MinimalApiEndpointConfigurator endpoint,
        EntityScheme entityScheme
    ) {
        GlobalConfiguration = globalConfiguration;
        Generate = generate;
        OperationType = operationType;
        OperationName = operationName;
        OperationGroup = operationGroup.GetName(entityScheme.EntityName, OperationName);
        OperationsSharedConfiguration = new(
            operationsSharedConfiguration.BusinessLogicFeatureName,
            operationsSharedConfiguration.BusinessLogicNamespaceForOperation,
            operationsSharedConfiguration.EndpointsNamespaceForFeature,
            entityScheme,
            OperationName,
            OperationGroup
        );
        Operation = operation.GetName(entityScheme.EntityName, OperationName);
        Handler = handler.GetName(entityScheme.EntityName, OperationName);
        var constructorParametersForRoute = entityScheme.PrimaryKeys.GetAsMethodCallArguments();
        Endpoint = new(
            Generate: endpoint.Generate,
            Name: endpoint.ClassName.GetName(entityScheme.EntityName, OperationName),
            FunctionName: endpoint.FunctionName.GetName(entityScheme.EntityName, OperationName),
            Route: endpoint.RouteConfigurator.GetRoute(
                entityScheme.EntityName.Name,
                OperationName,
                constructorParametersForRoute
            )
        );
    }
}