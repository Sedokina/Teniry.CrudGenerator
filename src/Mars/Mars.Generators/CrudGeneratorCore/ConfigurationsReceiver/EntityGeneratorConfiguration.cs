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

    public OperationWithReturnValueCustomization CreateOperation { get; set; }
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

public class OperationWithReturnValueCustomization
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