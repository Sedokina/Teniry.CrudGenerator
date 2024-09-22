using Mars.Generators.ApplicationGenerators.Configurations.Global;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations.TypedBuiltConfigurations;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations;

internal class CqrsOperationWithoutReturnValueGeneratorConfiguration
{
    public GlobalCqrsGeneratorConfiguration GlobalConfiguration { get; set; } = null!;
    public CqrsOperationsSharedConfiguration OperationsSharedConfiguration { get; set; } = null!;
    public CqrsOperationType OperationType { get; set; }
    public string FunctionName { get; set; } = "";
    public FileTemplateBasedOperationConfiguration Operation { get; set; } = null!;
    public FileTemplateBasedOperationConfiguration Handler { get; set; } = null!;
    public MinimalApiEndpointConfiguration Endpoint { get; set; } = null!;
}