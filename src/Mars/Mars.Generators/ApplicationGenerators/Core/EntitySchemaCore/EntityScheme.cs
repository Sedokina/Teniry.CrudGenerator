using System.Collections.Generic;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.Properties;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;

public class EntityScheme
{
    public ISymbol EntitySymbol { get; }
    public EntityName EntityName { get; set; }
    public EntityTitle EntityTitle { get; set; }
    public string EntityNamespace { get; set; }
    public List<EntityProperty> Properties { get; set; }
    public List<EntityProperty> PrimaryKeys { get; }
    public List<EntityProperty> NotPrimaryKeys { get; }
    public List<EntityProperty> SortableProperties { get; }

    public EntityScheme(
        ISymbol entitySymbol,
        EntityName entityName,
        EntityTitle entityTitle,
        string entityNamespace,
        List<EntityProperty> properties,
        List<EntityProperty> primaryKeys,
        List<EntityProperty> notPrimaryKeys,
        List<EntityProperty> sortableProperties)
    {
        EntitySymbol = entitySymbol;
        EntityName = entityName;
        EntityTitle = entityTitle;
        EntityNamespace = entityNamespace;
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
