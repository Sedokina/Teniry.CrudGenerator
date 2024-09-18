using System.Collections.Generic;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.Properties;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;

public class EntityScheme
{
    public ISymbol EntitySymbol { get; }
    public string EntityName { get; set; }
    public string EntityNamePlural { get; set; }
    public string EntityTitle { get; set; }
    public string EntityTitlePlural { get; set; }
    public string EntityNamespace { get; set; }
    public List<EntityProperty> Properties { get; set; }
    public List<EntityProperty> PrimaryKeys { get; }
    public List<EntityProperty> NotPrimaryKeys { get; }
    public List<EntityProperty> SortableProperties { get; }

    public EntityScheme(
        ISymbol entitySymbol,
        string entityName,
        string entityNamePlural,
        string entityTitle,
        string entityTitlePlural,
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