using System;
using System.Collections.Generic;
using System.Linq;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator.Operations;
using Microsoft.CodeAnalysis;

namespace ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator;

internal class InternalEntityGeneratorConfiguration
{
    public InternalEntityClassMetadata ClassMetadata { get; set; }
    public string? Title { get; set; }
    public string? TitlePlural { get; set; }
    public EntityDefaultSort? DefaultSort { get; set; }
    public InternalEntityGeneratorCreateOperationConfiguration? CreateOperation { get; set; }
    public InternalEntityGeneratorDeleteOperationConfiguration? DeleteOperation { get; set; }
    public InternalEntityGeneratorUpdateOperationConfiguration? UpdateOperation { get; set; }
    public InternalEntityGeneratorGetByIdOperationConfiguration? GetByIdOperation { get; set; }
    public InternalEntityGeneratorGetListOperationConfiguration? GetListOperation { get; set; }
}

internal record InternalEntityClassMetadata
{
    public string ClassName { get; set; }
    public string ContainingNamespace { get; set; }

    public string ContainingAssembly { get; set; }

    public EquatableList<InternalEntityClassPropertyMetadata> Properties { get; set; }

    public InternalEntityClassMetadata(
        string className,
        string containingNamespace,
        string containingAssembly,
        EquatableList<InternalEntityClassPropertyMetadata> properties)
    {
        ClassName = className;
        ContainingNamespace = containingNamespace;
        ContainingAssembly = containingAssembly;
        Properties = properties;
    }
}

internal record InternalEntityClassPropertyMetadata(
    string PropertyName,
    string TypeName,
    string TypeMetadataName,
    SpecialType SpecialType,
    bool IsSimpleType,
    bool IsNullable)
{
    public string PropertyName { get; set; } = PropertyName;
    public string TypeName { get; set; } = TypeName;
    public string TypeMetadataName { get; set; } = TypeMetadataName;
    public SpecialType SpecialType { get; set; } = SpecialType;
    public bool IsSimpleType { get; set; } = IsSimpleType;
    public bool IsNullable { get; set; } = IsNullable;

    public bool IsRangeType()
    {
        if (SpecialType == SpecialType.System_Boolean ||
            SpecialType == SpecialType.System_Char ||
            SpecialType == SpecialType.System_String ||
            TypeMetadataName == "Guid")
        {
            return false;
        }

        return IsSimpleType;
    }
}

public class EquatableList<T> : List<T>, IEquatable<EquatableList<T>>
{
    public EquatableList() : base() { }
    public EquatableList(IEnumerable<T> collection) : base(collection) { }

    public bool Equals(EquatableList<T>? other)
    {
        if (other is null || Count != other.Count)
        {
            return false;
        }

        for (var i = 0; i < Count; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as EquatableList<T>);
    }

    public override int GetHashCode()
    {
        return this.Select(item => item?.GetHashCode() ?? 0).Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(EquatableList<T> list1, EquatableList<T> list2)
    {
        return ReferenceEquals(list1, list2) || (list1 is not null && list2 is not null && list1.Equals(list2));
    }

    public static bool operator !=(EquatableList<T> list1, EquatableList<T> list2)
    {
        return !(list1 == list2);
    }
}