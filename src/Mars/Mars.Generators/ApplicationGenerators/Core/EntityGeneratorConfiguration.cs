using System;
using System.Linq.Expressions;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;

namespace Mars.Generators.ApplicationGenerators.Core;

public class EntityGeneratorConfiguration
{
}

public class EntityGeneratorConfiguration<TEntity> : EntityGeneratorConfiguration where TEntity : class
{
    public EntityTitle? Title { get; set; }
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