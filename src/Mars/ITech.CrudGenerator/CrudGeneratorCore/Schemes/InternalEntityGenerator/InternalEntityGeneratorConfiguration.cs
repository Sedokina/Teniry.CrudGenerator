using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;

namespace ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator;

internal class InternalEntityGeneratorConfiguration
{
    public string? Title { get; set; }
    public string? TitlePlural { get; set; }
    public EntityDefaultSort? DefaultSort { get; set; }
    public InternalEntityGeneratorCreateOperationConfiguration? CreateOperation { get; set; }
    public InternalEntityGeneratorDeleteOperationConfiguration? DeleteOperation { get; set; }
    public InternalEntityGeneratorUpdateOperationConfiguration? UpdateOperation { get; set; }
    public InternalEntityGeneratorGetByIdOperationConfiguration? GetByIdOperation { get; set; }
    public InternalEntityGeneratorGetListOperationConfiguration? GetListOperation { get; set; }
}

internal class InternalEntityGeneratorCreateOperationConfiguration
{
    public bool? Generate { get; set; }
    public string? Operation { get; set; }
    public string? OperationGroup { get; set; }
    public string? CommandName { get; set; }
    public string? DtoName { get; set; }
    public string? HandlerName { get; set; }
    public string? EndpointClassName { get; set; }
    public string? EndpointFunctionName { get; set; }
    public string? RouteName { get; set; }
    public bool? GenerateEndpoint { get; set; }
}

internal class InternalEntityGeneratorDeleteOperationConfiguration
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
}

internal class InternalEntityGeneratorUpdateOperationConfiguration
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

internal class InternalEntityGeneratorGetByIdOperationConfiguration
{
    public bool? Generate { get; set; }
    public string? Operation { get; set; }
    public string? OperationGroup { get; set; }
    public string? QueryName { get; set; }
    public string? DtoName { get; set; }
    public string? HandlerName { get; set; }
    public string? EndpointClassName { get; set; }
    public string? EndpointFunctionName { get; set; }
    public string? RouteName { get; set; }
    public bool? GenerateEndpoint { get; set; }
}

internal class InternalEntityGeneratorGetListOperationConfiguration
{
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