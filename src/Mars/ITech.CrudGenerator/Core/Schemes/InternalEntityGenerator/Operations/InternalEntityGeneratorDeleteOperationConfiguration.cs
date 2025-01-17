namespace ITech.CrudGenerator.Core.Schemes.InternalEntityGenerator.Operations;

internal record InternalEntityGeneratorDeleteOperationConfiguration {
    public bool? Generate { get; set; }
    public string? Operation { get; set; }
    public string? OperationGroup { get; set; }
    public string? CommandName { get; set; }
    public string? HandlerName { get; set; }
    public string? EndpointClassName { get; set; }
    public string? EndpointFunctionName { get; set; }
    public string? RouteName { get; set; }
    public bool? GenerateEndpoint { get; set; }
}