using System;
using System.Linq.Expressions;

namespace Teniry.CrudGenerator.Abstractions.Configuration;

public class EntityGeneratorDefaultSort<TEntity> where TEntity : class {
    public string Direction { get; set; }
    public Expression<Func<TEntity, object>> Name { get; set; }

    public EntityGeneratorDefaultSort(string direction, Expression<Func<TEntity, object>> name) {
        Direction = direction;
        Name = name;
    }
}