using System.Collections.Generic;
using Teniry.CrudGenerator.Abstractions.DbContext;
using Teniry.CrudGenerator.Core.Schemes.Entity.FilterExpressions.Core;

namespace Teniry.CrudGenerator.Core.Schemes.DbContext;

internal class DbContextScheme {
    private readonly Dictionary<FilterType, FilterExpression> _filterExpressions;
    public string DbContextNamespace { get; }
    public string DbContextName { get; }
    public DbContextDbProvider Provider { get; }

    public DbContextScheme(
        string dbContextNamespace,
        string dbContextName,
        DbContextDbProvider provider,
        Dictionary<FilterType, FilterExpression> filterExpressions
    ) {
        _filterExpressions = filterExpressions;
        DbContextNamespace = dbContextNamespace;
        DbContextName = dbContextName;
        Provider = provider;
    }

    public FilterExpression GetFilterExpression(FilterType filterType) {
        return _filterExpressions[filterType];
    }

    public bool ContainsFilter(FilterType filterType) {
        return _filterExpressions.ContainsKey(filterType);
    }

    protected bool Equals(DbContextScheme other) {
        return DbContextNamespace == other.DbContextNamespace &&
            DbContextName == other.DbContextName &&
            Provider == other.Provider;
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((DbContextScheme)obj);
    }

    public override int GetHashCode() {
        unchecked {
            var hashCode = DbContextNamespace.GetHashCode();
            hashCode = hashCode * 397 ^ DbContextName.GetHashCode();
            hashCode = hashCode * 397 ^ (int)Provider;

            return hashCode;
        }
    }
}