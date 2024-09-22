using Mars.Generators.CrudGeneratorCore.Configurations.Global;
using Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;
using Mars.Generators.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using Mars.Generators.CrudGeneratorCore.Schemes.Entity;
using Mars.Generators.CrudGeneratorCore.Schemes.Entity.Formatters;

namespace Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders;

internal class CqrsOperationWithoutReturnValueConfigurationBuilder
{
    public GlobalCqrsGeneratorConfigurationBuilder GlobalConfiguration { get; set; } = null!;
    public CqrsOperationsSharedConfigurationBuilder OperationsSharedConfiguration { get; set; } = null!;
    public CqrsOperationType OperationType { get; set; }
    public string OperationName { get; set; } = "";
    public NameConfigurationBuilder OperationGroup { get; set; } = null!;
    public FileTemplateBasedOperationConfigurationBuilder Operation { get; set; } = null!;
    public FileTemplateBasedOperationConfigurationBuilder Handler { get; set; } = null!;
    public MinimalApiEndpointConfigurationBuilder Endpoint { get; set; } = null!;

    protected void Init(CqrsOperationWithoutReturnValueGeneratorConfiguration configuration, EntityScheme entityScheme)
    {
        configuration.GlobalConfiguration = GlobalConfiguration.Build();
        configuration.OperationType = CqrsOperationType.Command;
        configuration.OperationName = OperationName;
        configuration.OperationGroup = OperationGroup.GetName(entityScheme.EntityName, configuration.OperationName);
        configuration.OperationsSharedConfiguration = OperationsSharedConfiguration
            .Build(entityScheme, configuration.OperationName, configuration.OperationGroup);

        configuration.Operation = new()
        {
            TemplatePath = Operation.TemplatePath
                .GetPath(configuration.GlobalConfiguration.TemplatesBasePath, configuration.OperationName),
            Name = Operation.NameConfigurationBuilder.GetName(entityScheme.EntityName, configuration.OperationName),
        };
        configuration.Handler = new()
        {
            TemplatePath = Handler.TemplatePath
                .GetPath(configuration.GlobalConfiguration.TemplatesBasePath, configuration.OperationName),
            Name = Handler.NameConfigurationBuilder.GetName(entityScheme.EntityName, configuration.OperationName),
        };

        var constructorParametersForRoute = entityScheme.PrimaryKeys.GetAsMethodCallParameters();
        configuration.Endpoint = new()
        {
            TemplatePath = Endpoint.TemplatePath
                .GetPath(configuration.GlobalConfiguration.TemplatesBasePath, configuration.OperationName),
            Name = Endpoint.NameConfigurationBuilder.GetName(entityScheme.EntityName, configuration.OperationName),
            FunctionName = Endpoint.FunctionName.GetName(entityScheme.EntityName, configuration.OperationName),
            Route = Endpoint.RouteConfigurationBuilder
                .GetRoute(entityScheme.EntityName.Name, configuration.OperationName, constructorParametersForRoute)
        };
    }

    public CqrsOperationWithoutReturnValueGeneratorConfiguration Build(EntityScheme entityScheme)
    {
        var result = new CqrsOperationWithoutReturnValueGeneratorConfiguration();
        Init(result, entityScheme);
        return result;
    }
}