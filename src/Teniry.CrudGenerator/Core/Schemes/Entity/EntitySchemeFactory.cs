using System;
using System.Linq;
using System.Text.RegularExpressions;
using Teniry.CrudGenerator.Core.Schemes.Entity.Extensions;
using Microsoft.CodeAnalysis;
using Pluralize.NET;
using Teniry.CrudGenerator.Core.Schemes.DbContext;
using Teniry.CrudGenerator.Core.Schemes.Entity.FilterExpressions.Core;
using Teniry.CrudGenerator.Core.Schemes.Entity.Properties;
using Teniry.CrudGenerator.Core.Schemes.InternalEntityGenerator;

namespace Teniry.CrudGenerator.Core.Schemes.Entity;

internal static class EntitySchemeFactory {
    private static readonly Pluralizer NamePluralizer = new();

    internal static EntityScheme Construct(
        InternalEntityGeneratorConfiguration internalEntityGeneratorConfiguration,
        DbContextScheme dbContextScheme
    ) {
        var classMetadata = internalEntityGeneratorConfiguration.ClassMetadata;
        var entityName = new EntityName(classMetadata.ClassName, GetPluralEntityName(classMetadata.ClassName));
        var entityTitle = CreateEntityTitle(internalEntityGeneratorConfiguration, entityName);
        var properties = GetEntityProperties(classMetadata.ClassName, classMetadata.Properties, dbContextScheme);

        return new(
            entityName,
            entityTitle,
            classMetadata.ContainingNamespace,
            classMetadata.ContainingAssembly,
            internalEntityGeneratorConfiguration.DefaultSort,
            properties,
            properties.Where(x => x.IsEntityId).ToList(),
            properties.Where(x => !x.IsEntityId).ToList(),
            properties.Where(x => x.CanBeSorted).ToList()
        );
    }

    private static EntityTitle CreateEntityTitle(
        InternalEntityGeneratorConfiguration internalEntityGeneratorConfiguration,
        EntityName entityName
    ) {
        var entityTitle = internalEntityGeneratorConfiguration.Title ?? GetTitleFromEntityName(entityName.ToString());
        var title = new EntityTitle(
            entityTitle,
            internalEntityGeneratorConfiguration.TitlePlural ?? GetPluralEntityTitle(entityTitle)
        );

        return title;
    }

    private static string GetPluralEntityName(string entityName) {
        var pluralEntityName = NamePluralizer.Pluralize(entityName);
        if (entityName.Equals(pluralEntityName)) {
            return $"{entityName}List";
        }

        return pluralEntityName;
    }

    private static string GetPluralEntityTitle(string entityTitle) {
        var pluralEntityName = NamePluralizer.Pluralize(entityTitle);
        if (entityTitle.Equals(pluralEntityName)) {
            return $"{entityTitle} list";
        }

        return pluralEntityName;
    }

    private static EquatableList<EntityProperty> GetEntityProperties(
        string className,
        EquatableList<InternalEntityClassPropertyMetadata> propertiesMetadata,
        DbContextScheme dbContextScheme
    ) {
        var result = new EquatableList<EntityProperty>();
        foreach (var propertyMetadata in propertiesMetadata) {
            if (!propertyMetadata.IsSimpleType) continue;

            // For DateTimeOffset and other date variations remove system from the property type declaration
            var propertyTypeName = propertyMetadata.TypeName.ToLower().StartsWith("system.") ?
                propertyMetadata.TypeMetadataName :
                propertyMetadata.TypeName;

            var defaultValue = propertyMetadata.SpecialType == SpecialType.System_String ? "\"\"" : null;

            var isPrimaryKey = IsPrimaryKey(className, propertyMetadata.PropertyName);
            var isForeignKey = IsForeignKey(propertyMetadata.PropertyName);
            var filterProperties = ConstructFilterProperties(
                isPrimaryKey || isForeignKey,
                propertyTypeName,
                propertyMetadata,
                dbContextScheme
            );
            result.Add(
                new(
                    propertyTypeName,
                    propertyMetadata.PropertyName,
                    propertyMetadata.PropertyName.ToLowerFirstChar(),
                    propertyMetadata.IsNullable,
                    defaultValue,
                    isPrimaryKey,
                    filterProperties,
                    propertyMetadata.IsSimpleType,
                    propertyMetadata.PropertyName.ToLowerFirstChar()
                )
            );
        }

        return result;
    }

    private static EntityFilterProperty[] ConstructFilterProperties(
        bool isForeignOrPrimaryKey,
        string propertyTypeName,
        InternalEntityClassPropertyMetadata propertyMetadata,
        DbContextScheme dbContextScheme
    ) {
        if (isForeignOrPrimaryKey) {
            var pluralPropertyName = NamePluralizer.Pluralize(propertyMetadata.PropertyName);
            if (dbContextScheme.ContainsFilter(FilterType.Contains)) {
                return [
                    new(
                        $"{propertyTypeName}[]?",
                        pluralPropertyName,
                        dbContextScheme.GetFilterExpression(FilterType.Contains)
                    )
                ];
            }

            return [];
        }

        if (!propertyMetadata.IsNullable) {
            propertyTypeName += "?";
        }

        if (propertyMetadata.IsRangeType()) {
            if (dbContextScheme.ContainsFilter(FilterType.GreaterThanOrEqual) &&
                dbContextScheme.ContainsFilter(FilterType.LessThan)) {
                return [
                    new(
                        propertyTypeName,
                        $"{propertyMetadata.PropertyName}From",
                        dbContextScheme.GetFilterExpression(FilterType.GreaterThanOrEqual)
                    ),
                    new(
                        propertyTypeName,
                        $"{propertyMetadata.PropertyName}To",
                        dbContextScheme.GetFilterExpression(FilterType.LessThan)
                    )
                ];
            }

            return [];
        }

        if (propertyMetadata.IsSimpleType) {
            if (propertyMetadata.SpecialType == SpecialType.System_String) {
                if (dbContextScheme.ContainsFilter(FilterType.Like)) {
                    return [
                        new(
                            propertyTypeName,
                            propertyMetadata.PropertyName,
                            dbContextScheme.GetFilterExpression(FilterType.Like)
                        )
                    ];
                }

                return [];
            }

            return [
                new(
                    propertyTypeName,
                    propertyMetadata.PropertyName,
                    dbContextScheme.GetFilterExpression(FilterType.Equals)
                )
            ];
        }

        return [];
    }

    private static bool IsPrimaryKey(string className, string propertyName) {
        return propertyName.Equals("id", StringComparison.CurrentCultureIgnoreCase) ||
            propertyName.Equals($"{className}Id", StringComparison.InvariantCultureIgnoreCase) ||
            propertyName.Equals("_id");
    }

    private static bool IsForeignKey(string propertyName) {
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
    private static string GetTitleFromEntityName(string entityName) {
        var regex = new Regex(
            "(?<=[A-Z])(?=[A-Z][a-z]) |  (?<=[^A-Z])(?=[A-Z]) | (?<=[A-Za-z])(?=[^A-Za-z])",
            RegexOptions.IgnorePatternWhitespace
        );

        return regex.Replace(entityName, " ").ToLowerAllButFirstChart();
    }
}