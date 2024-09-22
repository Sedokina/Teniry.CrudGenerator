using Mars.Generators.ApplicationGenerators.Configurations.Global;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.TypedConfigurations;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations;

public class CqrsOperationWithoutReturnValueGeneratorConfiguration
{
    public GlobalCqrsGeneratorConfiguration GlobalConfiguration { get; set; }
    public CqrsOperationsSharedConfiguration OperationsSharedConfiguration { get; set; }
    public CqrsOperationType OperationType { get; set; }
    public string FunctionName { get; set; }
    public FileTemplateBasedOperationConfiguration Operation { get; set; }
    public FileTemplateBasedOperationConfiguration Handler { get; set; }
    public MinimalApiEndpointConfiguration Endpoint { get; set; }
}

public class FileTemplateBasedOperationConfiguration
{
    public string TemplatePath { get; set; }
    public string Name { get; set; }
}

public class MinimalApiEndpointConfiguration : FileTemplateBasedOperationConfiguration
{
    public EndpointRouteConfiguration RouteConfiguration { get; set; } = null!;
}