using System.Collections.Generic;
using ITech.CrudGenerator.Core.Schemes.Entity.Properties;

namespace ITech.CrudGenerator.Core.Schemes.Entity;

internal class EntityScheme {
    public EntityName EntityName { get; }
    public EntityTitle EntityTitle { get; }
    public string EntityNamespace { get; }
    public string ContainingAssembly { get; }
    public EntityDefaultSort? DefaultSort { get; }
    public EquatableList<EntityProperty> Properties { get; }
    public List<EntityProperty> PrimaryKeys { get; }
    public List<EntityProperty> NotPrimaryKeys { get; }
    public List<EntityProperty> SortableProperties { get; }

    public EntityScheme(
        EntityName entityName,
        EntityTitle entityTitle,
        string entityNamespace,
        string containingAssembly,
        EntityDefaultSort? defaultSort,
        EquatableList<EntityProperty> properties,
        List<EntityProperty> primaryKeys,
        List<EntityProperty> notPrimaryKeys,
        List<EntityProperty> sortableProperties
    ) {
        EntityName = entityName;
        EntityTitle = entityTitle;
        EntityNamespace = entityNamespace;
        ContainingAssembly = containingAssembly;
        DefaultSort = defaultSort;
        Properties = properties;
        PrimaryKeys = primaryKeys;
        NotPrimaryKeys = notPrimaryKeys;
        SortableProperties = sortableProperties;
    }

    protected bool Equals(EntityScheme other) {
        return EntityName.Equals(other.EntityName) &&
            EntityTitle.Equals(other.EntityTitle) &&
            EntityNamespace == other.EntityNamespace &&
            ContainingAssembly == other.ContainingAssembly &&
            Equals(DefaultSort, other.DefaultSort) &&
            Properties.Equals(other.Properties);
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((EntityScheme)obj);
    }

    public override int GetHashCode() {
        unchecked {
            var hashCode = EntityName.GetHashCode();
            hashCode = (hashCode * 397) ^ EntityTitle.GetHashCode();
            hashCode = (hashCode * 397) ^ EntityNamespace.GetHashCode();
            hashCode = (hashCode * 397) ^ ContainingAssembly.GetHashCode();
            hashCode = (hashCode * 397) ^ (DefaultSort != null ? DefaultSort.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ Properties.GetHashCode();

            return hashCode;
        }
    }
}

internal record EntityName(string Name, string PluralName) {
    public string Name { get; } = Name;
    public string PluralName { get; } = PluralName;

    public override string ToString() {
        return Name;
    }
}

internal record EntityTitle(string Title, string PluralTitle) {
    public string Title { get; } = Title;
    public string PluralTitle { get; } = PluralTitle;

    public override string ToString() {
        return Title;
    }
}

internal record EntityDefaultSort(string Direction, string PropertyName) {
    public string Direction { get; } = Direction;
    public string PropertyName { get; } = PropertyName;
}