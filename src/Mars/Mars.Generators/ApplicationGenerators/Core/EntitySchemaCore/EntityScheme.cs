using System.Collections.Generic;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.Properties;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;

internal class EntityScheme
{
    public ISymbol EntitySymbol { get; }
    public EntityName EntityName { get; set; }
    public EntityTitle EntityTitle { get; set; }
    public string EntityNamespace { get; set; }
    public EntityDefaultSort? DefaultSort { get; set; }
    public List<EntityProperty> Properties { get; set; }
    public List<EntityProperty> PrimaryKeys { get; }
    public List<EntityProperty> NotPrimaryKeys { get; }
    public List<EntityProperty> SortableProperties { get; }

    public EntityScheme(
        ISymbol entitySymbol,
        EntityName entityName,
        EntityTitle entityTitle,
        string entityNamespace,
        EntityDefaultSort defaultSort,
        List<EntityProperty> properties,
        List<EntityProperty> primaryKeys,
        List<EntityProperty> notPrimaryKeys,
        List<EntityProperty> sortableProperties)
    {
        EntitySymbol = entitySymbol;
        EntityName = entityName;
        EntityTitle = entityTitle;
        EntityNamespace = entityNamespace;
        DefaultSort = defaultSort;
        Properties = properties;
        PrimaryKeys = primaryKeys;
        NotPrimaryKeys = notPrimaryKeys;
        SortableProperties = sortableProperties;
    }
}

public class EntityName(string name, string pluralName)
{
    public string Name { get; set; } = name;
    public string PluralName { get; set; } = pluralName;

    public override string ToString()
    {
        return Name;
    }
}

public class EntityTitle(string title, string pluralTitle)
{
    public string Title { get; set; } = title;
    public string PluralTitle { get; set; } = pluralTitle;

    public override string ToString()
    {
        return Title;
    }
}

internal class EntityDefaultSort(string direction, string propertyName)
{
    public string Direction { get; set; } = direction;
    public string PropertyName { get; set; } = propertyName;
}