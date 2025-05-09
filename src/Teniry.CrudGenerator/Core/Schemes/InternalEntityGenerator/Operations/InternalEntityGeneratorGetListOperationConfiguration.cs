namespace Teniry.CrudGenerator.Core.Schemes.InternalEntityGenerator.Operations;

internal record InternalEntityGeneratorGetListOperationConfiguration {
    public bool? Generate { get; set; }
    public string? Operation { get; set; }
    public string? OperationGroup { get; set; }
    public string? QueryName { get; set; }
    public string? DtoName { get; set; }
    public string? ListItemDtoName { get; set; }
    public string? FilterName { get; set; }
    public string? HandlerName { get; set; }
    public string? EndpointClassName { get; set; }
    public string? EndpointFunctionName { get; set; }
    public string? RouteName { get; set; }
    public bool? GenerateEndpoint { get; set; }
}