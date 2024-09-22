using Mars.Generators.CrudGeneratorCore.Schemes.Entity;

namespace Mars.Generators.CrudGeneratorCore.Schemes.EntityCustomization;

internal class EntityCustomizationScheme
{
    public string? Title { get; set; }
    public string? TitlePlural { get; set; }
    public EntityDefaultSort? DefaultSort { get; set; }
    public EntityCustomizationCreateOperationScheme CreateOperation { get; set; }
}

internal class EntityCustomizationCreateOperationScheme
{
    public bool? Generate { get; set; }
    public string? OperationType { get; set; }
    public string? OperationGroup { get; set; }
    public string? OperationName { get; set; }
    public string? DtoName { get; set; }
    public string? HandlerName { get; set; }
    public string? EndpointName { get; set; }
    public bool? GenerateEndpoint { get; set; }
}