namespace ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator.Operations;

internal record InternalEntityGeneratorUpdateOperationConfiguration
{
    public bool? Generate { get; set; }
    public string? Operation { get; set; }
    public string? OperationGroup { get; set; }
    public string? CommandName { get; set; }
    public string? HandlerName { get; set; }
    public string? EndpointClassName { get; set; }
    public string? EndpointFunctionName { get; set; }
    public string? RouteName { get; set; }
    public bool? GenerateEndpoint { get; set; }
    public string? ViewModelName { get; set; }
}