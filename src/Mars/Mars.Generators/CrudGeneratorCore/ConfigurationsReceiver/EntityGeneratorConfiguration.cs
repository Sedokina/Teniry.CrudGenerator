using System;
using System.Linq.Expressions;

namespace Mars.Generators.CrudGeneratorCore.ConfigurationsReceiver;

internal static class TypeNamesForAnalyzers
{
    public const string EntityGeneratorConfiguration = "EntityGeneratorConfiguration";
    public const string EntityGeneratorDefaultSort = "EntityGeneratorDefaultSort";
}

public abstract class EntityGeneratorConfiguration<TEntity> where TEntity : class
{
    public string? Title { get; set; }
    public string? TitlePlural { get; set; }
    public EntityGeneratorDefaultSort<TEntity>? DefaultSort { get; set; }
    public EntityGeneratorCreateOperationConfiguration? CreateOperation { get; set; }
    public EntityGeneratorDeleteOperationConfiguration? DeleteOperation { get; set; }
    public EntityGeneratorUpdateOperationConfiguration? UpdateOperation { get; set; }
    public EntityGeneratorGetByIdOperationConfiguration? GetByIdOperation { get; set; }
    public EntityGeneratorGetListOperationConfiguration? GetListOperation { get; set; }
}

public class EntityGeneratorDefaultSort<TEntity> where TEntity : class
{
    public string Direction { get; set; }
    public Expression<Func<TEntity, object>> Name { get; set; }

    public EntityGeneratorDefaultSort(string direction, Expression<Func<TEntity, object>> name)
    {
        Direction = direction;
        Name = name;
    }
}

public sealed class EntityGeneratorCreateOperationConfiguration
{
    public bool? Generate { get; set; }
    public string? OperationType { get; set; }
    public string? OperationGroup { get; set; }
    public string? OperationName { get; set; }
    public string? DtoName { get; set; }
    public string? HandlerName { get; set; }
    public bool? GenerateEndpoint { get; set; }
    public string? EndpointClassName { get; set; }
    public string? EndpointFunctionName { get; set; }
    public string? RouteName { get; set; }
}

public sealed class EntityGeneratorDeleteOperationConfiguration
{
    public bool? Generate { get; set; }
    public string? OperationType { get; set; }
    public string? OperationGroup { get; set; }
    public string? OperationName { get; set; }
    public string? HandlerName { get; set; }
    public bool? GenerateEndpoint { get; set; }
    public string? EndpointClassName { get; set; }
    public string? EndpointFunctionName { get; set; }
    public string? RouteName { get; set; }
}

public sealed class EntityGeneratorUpdateOperationConfiguration
{
    public bool? Generate { get; set; }
    public string? OperationType { get; set; }
    public string? OperationGroup { get; set; }
    public string? OperationName { get; set; }
    public string? HandlerName { get; set; }
    public bool? GenerateEndpoint { get; set; }
    public string? EndpointClassName { get; set; }
    public string? EndpointFunctionName { get; set; }
    public string? RouteName { get; set; }
}

public sealed class EntityGeneratorGetByIdOperationConfiguration
{
    public bool? Generate { get; set; }
    public string? OperationType { get; set; }
    public string? OperationGroup { get; set; }
    public string? OperationName { get; set; }
    public string? DtoName { get; set; }
    public string? HandlerName { get; set; }
    public bool? GenerateEndpoint { get; set; }
    public string? EndpointClassName { get; set; }
    public string? EndpointFunctionName { get; set; }
    public string? RouteName { get; set; }
}

public sealed class EntityGeneratorGetListOperationConfiguration
{
    public bool? Generate { get; set; }
    public string? OperationType { get; set; }
    public string? OperationGroup { get; set; }
    public string? OperationName { get; set; }
    public string? DtoName { get; set; }
    public string? DtoListItem { get; set; }
    public string? FilterName { get; set; }
    public string? HandlerName { get; set; }
    public bool? GenerateEndpoint { get; set; }
    public string? EndpointClassName { get; set; }
    public string? EndpointFunctionName { get; set; }
    public string? RouteName { get; set; }
}