using ITech.CrudGenerator.Core.Configurations.Configurators;
using ITech.CrudGenerator.Core.Configurations.Crud.TypedConfigurations;
using ITech.CrudGenerator.Core.Configurations.Global;
using ITech.CrudGenerator.Core.Configurations.Shared;
using ITech.CrudGenerator.Core.Schemes.Entity;
using ITech.CrudGenerator.Core.Schemes.Entity.Formatters;

namespace ITech.CrudGenerator.Core.Configurations.Crud;

internal record CqrsOperationWithoutReturnValueGeneratorConfiguration
{
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
        EntityScheme entityScheme)
    {
        GlobalConfiguration = globalConfiguration;
        Generate = generate;
        OperationType = operationType;
        OperationName = operationName;
        OperationGroup = operationGroup.GetName(entityScheme.EntityName, OperationName);
        OperationsSharedConfiguration = new CqrsOperationsSharedConfiguration(
            businessLogicFeatureName: operationsSharedConfiguration.BusinessLogicFeatureName,
            businessLogicNamespaceForOperation: operationsSharedConfiguration.BusinessLogicNamespaceForOperation,
            endpointsNamespaceForFeature: operationsSharedConfiguration.EndpointsNamespaceForFeature,
            entityScheme: entityScheme,
            operationName: OperationName,
            operationGroup: OperationGroup
        );
        Operation = operation.GetName(entityScheme.EntityName, OperationName);
        Handler = handler.GetName(entityScheme.EntityName, OperationName);
        var constructorParametersForRoute = entityScheme.PrimaryKeys.GetAsMethodCallArguments();
        Endpoint = new MinimalApiEndpointConfiguration(
            Generate: endpoint.Generate,
            Name: endpoint.ClassName.GetName(entityScheme.EntityName, OperationName),
            FunctionName: endpoint.FunctionName.GetName(entityScheme.EntityName, OperationName),
            Route: endpoint.RouteConfigurator.GetRoute(entityScheme.EntityName.Name, OperationName,
                constructorParametersForRoute));
    }
}