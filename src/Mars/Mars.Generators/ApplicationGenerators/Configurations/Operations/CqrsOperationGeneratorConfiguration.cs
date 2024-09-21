using Mars.Generators.ApplicationGenerators.Configurations.Global;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.TypedConfigurations;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations;

public class CqrsOperationGeneratorConfiguration
{
    public GlobalCqrsGeneratorConfiguration GlobalConfiguration { get; set; }
    public CqrsOperationsSharedConfiguration OperationsSharedConfiguration { get; set; }
    public CqrsOperationType OperationType { get; set; }
    public NameConfiguration FunctionName { get; set; }
    public FileTemplateBasedOperationConfiguration Operation { get; set; }
    public FileTemplateBasedOperationConfiguration Handler { get; set; }
    public MinimalApiEndpointConfiguration Endpoint { get; set; }

    protected virtual BuildConfiguration(EntityName entityName)
    {
        
    }
    
    public CqrsOperationGeneratorConfigurationBuilt Build(EntityName entityName)
    {
        return new CqrsOperationGeneratorConfigurationBuilt
        {
            GlobalConfiguration = GlobalConfiguration,
            OperationsSharedConfiguration = OperationsSharedConfiguration,
            OperationType = CqrsOperationType.Command,
            FunctionName = FunctionName.GetName(entityName),
            Operation = new()
            {
                TemplatePath = Operation.TemplatePath,
                Name = Operation.NameConfiguration.GetName(entityName),
            },
            Handler = new()
            {
                TemplatePath = Handler.TemplatePath,
                Name = Handler.NameConfiguration.GetName(entityName),
            },
            Endpoint = new()
            {
                TemplatePath = Endpoint.TemplatePath,
                Name = Endpoint.NameConfiguration.GetName(entityName),
            }
        };
    }
}

public class CqrsOperationGeneratorConfigurationBuilt
{
    public GlobalCqrsGeneratorConfiguration GlobalConfiguration { get; set; }
    public CqrsOperationsSharedConfiguration OperationsSharedConfiguration { get; set; }
    public CqrsOperationType OperationType { get; set; }
    public string FunctionName { get; set; }
    public FileTemplateBasedOperationConfigurationBuilt Operation { get; set; }
    public FileTemplateBasedOperationConfigurationBuilt Handler { get; set; }
    public FileTemplateBasedOperationConfigurationBuilt Endpoint { get; set; }
}

public class FileTemplateBasedOperationConfigurationBuilt
{
    public string TemplatePath { get; set; }
    public string Name { get; set; }
}