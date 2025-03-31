using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Teniry.CrudGenerator.SampleApi.Mongo;

public class MongoEfIntIdSequenceGenerator<T> : ValueGenerator<int> where T : class {
    public override bool GeneratesTemporaryValues => false;

    public override int Next(EntityEntry entry) {
        var currInd = entry.Context.Set<T>().Count();

        return currInd + 1;
    }
}