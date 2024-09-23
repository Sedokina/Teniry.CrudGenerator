using Mars.Generators.CrudGeneratorCore.Schemes.Entity;

namespace Mars.Generators.CrudGeneratorCore.Schemes.EntityCustomization;

internal class EntityCustomizationScheme
{
    public string? Title { get; set; }
    public string? TitlePlural { get; set; }
    public EntityDefaultSort? DefaultSort { get; set; }
    public EntityCreateOperationCustomizationScheme? CreateOperation { get; set; }
    public EntityDeleteOperationCustomizationScheme? DeleteOperation { get; set; }
    public EntityUpdateOperationCustomizationScheme? UpdateOperation { get; set; }
    public EntityGetByIdOperationCustomizationScheme? GetByIdOperation { get; set; }
    public EntityGetListOperationCustomizationScheme? GetListOperation { get; set; }
}

internal class EntityCreateOperationCustomizationScheme
{
    public bool? Generate { get; set; }
    public string? OperationType { get; set; }
    public string? OperationGroup { get; set; }
    public string? OperationName { get; set; }
    public string? DtoName { get; set; }
    public string? HandlerName { get; set; }
    public string? EndpointClassName { get; set; }
    public string? EndpointFunctionName { get; set; }
    public string? RouteName { get; set; }
    public bool? GenerateEndpoint { get; set; }
}

internal class EntityDeleteOperationCustomizationScheme
{
    public bool? Generate { get; set; }
    public string? OperationType { get; set; }
    public string? OperationGroup { get; set; }
    public string? OperationName { get; set; }
    public string? HandlerName { get; set; }
    public string? EndpointClassName { get; set; }
    public string? EndpointFunctionName { get; set; }
    public string? RouteName { get; set; }
    public bool? GenerateEndpoint { get; set; }
}

internal class EntityUpdateOperationCustomizationScheme
{
    public bool? Generate { get; set; }
    public string? OperationType { get; set; }
    public string? OperationGroup { get; set; }
    public string? OperationName { get; set; }
    public string? HandlerName { get; set; }
    public string? EndpointClassName { get; set; }
    public string? EndpointFunctionName { get; set; }
    public string? RouteName { get; set; }
    public bool? GenerateEndpoint { get; set; }
}

internal class EntityGetByIdOperationCustomizationScheme
{
    public bool? Generate { get; set; }
    public string? OperationType { get; set; }
    public string? OperationGroup { get; set; }
    public string? OperationName { get; set; }
    public string? DtoName { get; set; }
    public string? HandlerName { get; set; }
    public string? EndpointClassName { get; set; }
    public string? EndpointFunctionName { get; set; }
    public string? RouteName { get; set; }
    public bool? GenerateEndpoint { get; set; }
}

internal class EntityGetListOperationCustomizationScheme
{
    public bool? Generate { get; set; }
    public string? OperationType { get; set; }
    public string? OperationGroup { get; set; }
    public string? OperationName { get; set; }
    public string? DtoName { get; set; }
    public string? DtoListItemName { get; set; }
    public string? FilterName { get; set; }
    public string? HandlerName { get; set; }
    public string? EndpointClassName { get; set; }
    public string? EndpointFunctionName { get; set; }
    public string? RouteName { get; set; }
    public bool? GenerateEndpoint { get; set; }
}