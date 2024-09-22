using System;
using System.Linq.Expressions;

namespace Mars.Generators.ApplicationGenerators.Core;

public abstract class EntityGeneratorConfiguration
{
}

public abstract class EntityGeneratorConfiguration<TEntity> : EntityGeneratorConfiguration where TEntity : class
{
    public string? Title { get; set; }
    public string? TitlePlural { get; set; }
    public EntityGeneratorDefaultSort<TEntity>? DefaultSort { get; set; }
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