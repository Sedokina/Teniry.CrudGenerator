using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.DbContext;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Core;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.Properties;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.EntityCustomization;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.Extensions;
using Microsoft.CodeAnalysis;
using Pluralize.NET;

namespace ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;

internal class EntitySchemeFactory
{
    internal static EntityScheme Construct(
        ISymbol symbol,
        EntityCustomizationScheme entityCustomizationScheme,
        DbContextScheme dbContextScheme)
    {
        var properties = GetEntityProperties(symbol, dbContextScheme);
        var entityName = new EntityName(symbol.Name, GetPluralEntityName(symbol.Name));
        var entityTitle = CreateEntityTitle(entityCustomizationScheme, entityName);
        return new EntityScheme(symbol,
            entityName,
            entityTitle,
            symbol.ContainingNamespace.ToString(),
            symbol.ContainingAssembly.Name,
            entityCustomizationScheme.DefaultSort,
            properties,
            properties.Where(x => x.IsEntityId).ToList(),
            properties.Where(x => !x.IsEntityId).ToList(),
            properties.Where(x => x.CanBeSorted).ToList());
    }

    private static EntityTitle CreateEntityTitle(
        EntityCustomizationScheme entityCustomizationScheme,
        EntityName entityName)
    {
        var entityTitle = entityCustomizationScheme.Title ?? GetTitleFromEntityName(entityName.ToString());
        var title = new EntityTitle(
            entityTitle,
            entityCustomizationScheme.TitlePlural ?? GetPluralEntityTitle(entityTitle));
        return title;
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

    private static string GetPluralEntityTitle(string entityTitle)
    {
        var pluralizer = new Pluralizer();
        var pluralEntityName = pluralizer.Pluralize(entityTitle);
        if (entityTitle.Equals(pluralEntityName))
        {
            return $"{entityTitle} list";
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

            var defaultValue = propertySymbol.Type.SpecialType == SpecialType.System_String ? "\"\"" : null;

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
                defaultValue,
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
        return propertyName.Equals("id", StringComparison.CurrentCultureIgnoreCase) ||
               propertyName.Equals($"{className}Id", StringComparison.InvariantCultureIgnoreCase) ||
               propertyName.Equals("_id");
    }

    private static bool IsForeignKey(string propertyName)
    {
        return propertyName.EndsWith("id", StringComparison.InvariantCultureIgnoreCase) ||
               propertyName.EndsWith("_id", StringComparison.CurrentCultureIgnoreCase);
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