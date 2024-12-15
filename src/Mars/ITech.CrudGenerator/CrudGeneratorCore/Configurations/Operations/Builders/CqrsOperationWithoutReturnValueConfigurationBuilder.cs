using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.Formatters;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;

internal class CqrsOperationWithoutReturnValueConfigurationBuilder
{
    public bool Generate { get; set; } = true;
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
        configuration.Generate = Generate;
        configuration.OperationType = OperationType;
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

        configuration.Endpoint = Endpoint
            .Build(entityScheme, configuration.GlobalConfiguration.TemplatesBasePath, configuration.OperationName);
    }

    public CqrsOperationWithoutReturnValueGeneratorConfiguration Build(EntityScheme entityScheme)
    {
        var result = new CqrsOperationWithoutReturnValueGeneratorConfiguration();
        Init(result, entityScheme);
        return result;
    }
}