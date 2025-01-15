namespace ITech.CrudGenerator.Core.Schemes.Entity.Properties;

internal class EntityProperty
{
    public string TypeName { get; set; }
    public string PropertyName { get; set; }

    // TODO: remove?
    public string PropertyNameAsMethodParameterName { get; set; }
    public bool IsNullable { get; }
    public bool IsEntityId { get; set; }
    public bool CanBeSorted { get; set; }
    public string SortKey { get; set; }
    public string? DefaultValue { get; set; }
    public EntityFilterProperty[] FilterProperties { get; set; }

    public EntityProperty(
        string typeName,
        string propertyName,
        string propertyNameAsMethodParameterName,
        bool isNullable,
        string? defaultValue,
        bool isEntityId,
        EntityFilterProperty[] filterProperties,
        bool canBeSorted,
        string sortKey)
    {
        TypeName = typeName;
        PropertyName = propertyName;
        PropertyNameAsMethodParameterName = propertyNameAsMethodParameterName;
        IsNullable = isNullable;
        DefaultValue = defaultValue;
        IsEntityId = isEntityId;
        FilterProperties = filterProperties;
        CanBeSorted = canBeSorted;
        SortKey = sortKey;
    }
}