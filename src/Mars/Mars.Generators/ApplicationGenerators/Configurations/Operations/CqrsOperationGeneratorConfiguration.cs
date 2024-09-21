using Mars.Generators.ApplicationGenerators.Configurations.Global;
using Mars.Generators.ApplicationGenerators.Configurations.Global.TypedConfigurations;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.TypedConfigurations;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations;

public class CqrsOperationGeneratorConfiguration
{
    public GlobalCqrsGeneratorConfiguration GlobalConfiguration { get; set; }
    public CqrsOperationType OperationType { get; set; }
    public NameConfiguration FunctionName { get; set; }
    public FileTemplateBasedOperationConfiguration Operation { get; set; }
    public FileTemplateBasedOperationConfiguration Handler { get; set; }
    public MinimalApiEndpointConfiguration Endpoint { get; set; }
}