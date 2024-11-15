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
    private readonly Pluralizer _pluralizer;

    public EntitySchemeFactory()
    {
        _pluralizer = new();
    }

    internal EntityScheme Construct(
        ISymbol symbol,
        EntityCustomizationScheme entityCustomizationScheme,
        DbContextScheme dbContextScheme)
    {
        var entityName = new EntityName(symbol.Name, GetPluralEntityName(symbol.Name));
        var entityTitle = CreateEntityTitle(entityCustomizationScheme, entityName);
        var properties = GetEntityProperties(symbol, dbContextScheme);
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

    private EntityTitle CreateEntityTitle(
        EntityCustomizationScheme entityCustomizationScheme,
        EntityName entityName)
    {
        var entityTitle = entityCustomizationScheme.Title ?? GetTitleFromEntityName(entityName.ToString());
        var title = new EntityTitle(
            entityTitle,
            entityCustomizationScheme.TitlePlural ?? GetPluralEntityTitle(entityTitle));
        return title;
    }

    private string GetPluralEntityName(string entityName)
    {
        var pluralEntityName = _pluralizer.Pluralize(entityName);
        if (entityName.Equals(pluralEntityName))
        {
            return $"{entityName}List";
        }

        return pluralEntityName;
    }

    private string GetPluralEntityTitle(string entityTitle)
    {
        var pluralEntityName = _pluralizer.Pluralize(entityTitle);
        if (entityTitle.Equals(pluralEntityName))
        {
            return $"{entityTitle} list";
        }

        return pluralEntityName;
    }

    private List<EntityProperty> GetEntityProperties(ISymbol symbol, DbContextScheme dbContextScheme)
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

    private EntityFilterProperty[] ConstructFilterProperties(
        bool isForeignOrPrimaryKey,
        string propertyTypeName,
        string propertyName,
        ITypeSymbol propertyType,
        DbContextScheme dbContextScheme)
    {
        if (isForeignOrPrimaryKey)
        {
            var pluralPropertyName = _pluralizer.Pluralize(propertyName);
            if (dbContextScheme.ContainsFilter(FilterType.Contains))
            {
                return
                [
                    new EntityFilterProperty(
                        $"{propertyTypeName}[]?",
                        pluralPropertyName,
                        dbContextScheme.GetFilterExpression(FilterType.Contains))
                ];
            }

            return [];
        }

        if (propertyType.NullableAnnotation != NullableAnnotation.Annotated)
        {
            propertyTypeName += "?";
        }

        if (propertyType.IsRangeType())
        {
            if (dbContextScheme.ContainsFilter(FilterType.GreaterThanOrEqual) &&
                dbContextScheme.ContainsFilter(FilterType.LessThan))
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

            return [];
        }

        if (propertyType.IsSimple())
        {
            if (propertyType.SpecialType == SpecialType.System_String)
            {
                if (dbContextScheme.ContainsFilter(FilterType.Like))
                {
                    return
                    [
                        new EntityFilterProperty(
                            propertyTypeName,
                            propertyName,
                            dbContextScheme.GetFilterExpression(FilterType.Like))
                    ];
                }

                return [];
            }

            return
            [
                new EntityFilterProperty(
                    propertyTypeName,
                    propertyName,
                    dbContextScheme.GetFilterExpression(FilterType.Equals))
            ];
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