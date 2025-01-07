using System.Collections.Immutable;
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

internal class InternalEntityClassMetadata
{
    public string ClassName { get; set; }
    public string ContainingNamespance { get; set; }
    public string ContainingAssembly { get; set; }
    public ImmutableArray<InternalEntityClassPropertyMetadata> Properties { get; set; }
}

internal class InternalEntityClassPropertyMetadata
{
    public string TypeName { get; set; }
    public string TypeMetadataName { get; set; }
    public string PropertyName { get; set; }
    public SpecialType SpecialType { get; set; }
    public bool IsSimpleType { get; set; }
    public bool IsNullable { get; set; }
    public bool IsRangeType { get; set; }
}