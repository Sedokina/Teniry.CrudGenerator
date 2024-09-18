namespace Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.Properties;

public class EntityProperty
{
    public string TypeName { get; set; }
    public string PropertyName { get; set; }
    public string PropertyNameAsMethodParameterName { get; set; }
    public bool IsEntityId { get; set; }
    public bool CanBeSorted { get; set; }
    public string SortKey { get; set; }
    public EntityFilterProperty[] FilterProperties { get; set; }

    public EntityProperty(
        string typeName,
        string propertyName,
        string propertyNameAsMethodParameterName,
        bool isEntityId,
        EntityFilterProperty[] filterProperties,
        bool canBeSorted,
        string sortKey)
    {
        TypeName = typeName;
        PropertyName = propertyName;
        PropertyNameAsMethodParameterName = propertyNameAsMethodParameterName;
        IsEntityId = isEntityId;
        FilterProperties = filterProperties;
        CanBeSorted = canBeSorted;
        SortKey = sortKey;
    }
}