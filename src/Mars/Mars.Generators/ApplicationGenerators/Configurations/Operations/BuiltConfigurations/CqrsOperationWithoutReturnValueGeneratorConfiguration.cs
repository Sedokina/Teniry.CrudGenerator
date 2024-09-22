using Mars.Generators.ApplicationGenerators.Configurations.Global;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations.TypedBuiltConfigurations;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations;

internal class CqrsOperationWithoutReturnValueGeneratorConfiguration
{
    public GlobalCqrsGeneratorConfiguration GlobalConfiguration { get; set; }
    public CqrsOperationsSharedConfiguration OperationsSharedConfiguration { get; set; }
    public CqrsOperationType OperationType { get; set; }
    public string FunctionName { get; set; }
    public FileTemplateBasedOperationConfiguration Operation { get; set; }
    public FileTemplateBasedOperationConfiguration Handler { get; set; }
    public MinimalApiEndpointConfiguration Endpoint { get; set; }
}