using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.FilterExpressions.Core;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.FilterExpressions.Expressions;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.Properties;
using Mars.Generators.ApplicationGenerators.Core.Extensions;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;

public class EntitySchemeFactory
{
    public static EntityScheme Construct(ISymbol symbol)
    {
        var properties = GetEntityProperties(symbol);
        return new EntityScheme(symbol,
            symbol.Name,
            GetTitleFromEntityName(symbol.Name),
            symbol.ContainingNamespace.ToString(),
            properties,
            properties.Where(x => x.IsEntityId).ToList(),
            properties.Where(x => !x.IsEntityId).ToList(),
            properties.Where(x => x.CanBeSorted).ToList());
    }

    private static List<EntityProperty> GetEntityProperties(ISymbol symbol)
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
            result.Add(new EntityProperty(
                propertyTypeName,
                propertySymbol.Name,
                propertySymbol.Name.ToLowerFirstChar(),
                isPrimaryKey,
                ConstructFilterProperties(
                    isPrimaryKey || isForeignKey,
                    propertyTypeName,
                    propertySymbol.Name,
                    propertySymbol.Type),
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
        ITypeSymbol propertyType)
    {
        if (isForeignOrPrimaryKey)
        {
            return [new EntityFilterProperty($"{propertyTypeName}[]?", propertyName, new ContainsFilterExpression())];
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
                    new GreaterThanOrEqualFilterExpression()),
                new EntityFilterProperty(
                    propertyTypeName,
                    $"{propertyName}To",
                    new LessThanFilterExpression())
            ];
        }

        if (propertyType.IsSimple())
        {
            FilterExpression filterExpression = propertyType.SpecialType == SpecialType.System_String
                ? new LikeFilterExpression()
                : new EqualsFilterExpression();
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