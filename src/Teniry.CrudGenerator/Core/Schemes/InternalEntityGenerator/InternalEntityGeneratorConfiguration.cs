using Microsoft.CodeAnalysis;
using Teniry.CrudGenerator.Core.Schemes.Entity;
using Teniry.CrudGenerator.Core.Schemes.InternalEntityGenerator.Operations;

namespace Teniry.CrudGenerator.Core.Schemes.InternalEntityGenerator;

internal record InternalEntityGeneratorConfiguration(InternalEntityClassMetadata ClassMetadata) {
    public InternalEntityClassMetadata ClassMetadata { get; set; } = ClassMetadata;
    public string? Title { get; set; }
    public string? TitlePlural { get; set; }
    public EntityDefaultSort? DefaultSort { get; set; }
    public InternalEntityGeneratorCreateOperationConfiguration? CreateOperation { get; set; }
    public InternalEntityGeneratorDeleteOperationConfiguration? DeleteOperation { get; set; }
    public InternalEntityGeneratorUpdateOperationConfiguration? UpdateOperation { get; set; }
    public InternalEntityGeneratorGetByIdOperationConfiguration? GetByIdOperation { get; set; }
    public InternalEntityGeneratorGetListOperationConfiguration? GetListOperation { get; set; }
}

internal record InternalEntityClassMetadata {
    public string ClassName { get; set; }
    public string ContainingNamespace { get; set; }

    public string ContainingAssembly { get; set; }

    public EquatableList<InternalEntityClassPropertyMetadata> Properties { get; set; }

    public InternalEntityClassMetadata(
        string className,
        string containingNamespace,
        string containingAssembly,
        EquatableList<InternalEntityClassPropertyMetadata> properties
    ) {
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
    bool IsNullable
) {
    public string PropertyName { get; set; } = PropertyName;
    public string TypeName { get; set; } = TypeName;
    public string TypeMetadataName { get; set; } = TypeMetadataName;
    public SpecialType SpecialType { get; set; } = SpecialType;
    public bool IsSimpleType { get; set; } = IsSimpleType;
    public bool IsNullable { get; set; } = IsNullable;

    public bool IsRangeType() {
        if (SpecialType == SpecialType.System_Boolean ||
            SpecialType == SpecialType.System_Char ||
            SpecialType == SpecialType.System_String ||
            TypeMetadataName == "Guid") {
            return false;
        }

        return IsSimpleType;
    }
}