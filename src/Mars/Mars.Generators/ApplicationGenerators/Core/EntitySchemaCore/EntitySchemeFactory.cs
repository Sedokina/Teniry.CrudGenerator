using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mars.Generators.ApplicationGenerators.Core.DbContextCore;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.FilterExpressions.Core;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.Properties;
using Mars.Generators.ApplicationGenerators.Core.Extensions;
using Microsoft.CodeAnalysis;
using Pluralize.NET;

namespace Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;

public class EntitySchemeFactory
{
    public static EntityScheme Construct(ISymbol symbol, DbContextScheme dbContextScheme)
    {
        var properties = GetEntityProperties(symbol, dbContextScheme);
        var entityName = new EntityName(symbol.Name, GetPluralEntityName(symbol.Name));
        var entityTitle = new EntityTitle(GetTitleFromEntityName(entityName.ToString()),
            GetTitleFromEntityName(entityName.PluralName));
        return new EntityScheme(symbol,
            entityName,
            entityTitle,
            symbol.ContainingNamespace.ToString(),
            properties,
            properties.Where(x => x.IsEntityId).ToList(),
            properties.Where(x => !x.IsEntityId).ToList(),
            properties.Where(x => x.CanBeSorted).ToList());
    }

    private static string GetPluralEntityName(string entityName)
    {
        var pluralizer = new Pluralizer();
        var pluralEntityName = pluralizer.Pluralize(entityName);
        if (entityName.Equals(pluralEntityName))
        {
            return $"{entityName}List";
        }

        return pluralEntityName;
    }

    private static List<EntityProperty> GetEntityProperties(ISymbol symbol, DbContextScheme dbContextScheme)
    {
        var propertiesOfClass = ((INamedTypeSymbol)symbol).GetMembers().OfType<IPropertySymbol>();
        var result = new List<EntityProperty>();
        foreach (var propertySymbol in propertiesOfClass)
        {
            if (!propertySymbol.Type.IsSimple()) continue;

            // For DateTimeOffset and other date variations remove system from the property type declaration
            var propertyTypeName = propertySymbol.Type.ToString().ToLower().StartsWith("system.")
                ? propertySymbol.Type.MetadataName
                : propertySymbol.Type.ToString();

            var isPrimaryKey = IsPrimaryKey(symbol.Name, propertySymbol.Name);
            var isForeignKey = IsForeignKey(propertySymbol.Name);
            var filterProperties = ConstructFilterProperties(
                isPrimaryKey || isForeignKey,
                propertyTypeName,
                propertySymbol.Name,
                propertySymbol.Type,
                dbContextScheme);
            result.Add(new EntityProperty(
                propertyTypeName,
                propertySymbol.Name,
                propertySymbol.Name.ToLowerFirstChar(),
                isPrimaryKey,
                filterProperties,
                propertySymbol.Type.IsSimple(),
                propertySymbol.Name.ToLowerFirstChar()
            ));
        }

        return result;
    }

    private static EntityFilterProperty[] ConstructFilterProperties(
        bool isForeignOrPrimaryKey,
        string propertyTypeName,
        string propertyName,
        ITypeSymbol propertyType,
        DbContextScheme dbContextScheme)
    {
        if (isForeignOrPrimaryKey)
        {
            return
            [
                new EntityFilterProperty(
                    $"{propertyTypeName}[]?",
                    propertyName,
                    dbContextScheme.GetFilterExpression(FilterType.Contains))
            ];
        }

        if (propertyType.NullableAnnotation != NullableAnnotation.Annotated)
        {
            propertyTypeName += "?";
        }

        if (propertyType.IsRangeType())
        {
            return
            [
                new EntityFilterProperty(
                    propertyTypeName,
                    $"{propertyName}From",
                    dbContextScheme.GetFilterExpression(FilterType.GreaterThanOrEqual)),
                new EntityFilterProperty(
                    propertyTypeName,
                    $"{propertyName}To",
                    dbContextScheme.GetFilterExpression(FilterType.LessThan))
            ];
        }

        if (propertyType.IsSimple())
        {
            FilterExpression filterExpression = propertyType.SpecialType == SpecialType.System_String
                ? dbContextScheme.GetFilterExpression(FilterType.Like)
                : dbContextScheme.GetFilterExpression(FilterType.Equals);
            return [new EntityFilterProperty(propertyTypeName, propertyName, filterExpression)];
        }

        return [];
    }


    private static bool IsPrimaryKey(string className, string propertyName)
    {
        var lower = propertyName.ToLower();
        return lower.Equals("id") || lower.Equals($"{className}id") || lower.Equals("_id");
    }

    private static bool IsForeignKey(string propertyName)
    {
        var lower = propertyName.ToLower();
        return lower.EndsWith("id") || lower.EndsWith("_id");
    }

    /// <summary>
    ///     Spread camel or pascal case string as separate words
    /// </summary>
    /// <param name="entityName"></param>
    /// <returns></returns>
    /// <example>
    ///     MyProjectClassName returns My project class name
    /// </example>
    private static string GetTitleFromEntityName(string entityName)
    {
        var regex = new Regex("(?<=[A-Z])(?=[A-Z][a-z]) |  (?<=[^A-Z])(?=[A-Z]) | (?<=[A-Za-z])(?=[^A-Za-z])",
            RegexOptions.IgnorePatternWhitespace);
        return regex.Replace(entityName, " ").ToLowerAllButFirstChart();
    }
}