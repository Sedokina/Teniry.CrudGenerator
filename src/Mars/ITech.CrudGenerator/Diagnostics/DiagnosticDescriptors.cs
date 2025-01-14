using ITech.CrudGenerator.Abstractions.DbContext;
using Microsoft.CodeAnalysis;

namespace ITech.CrudGenerator.Diagnostics;

// Diagnostic models were taken from https://andrewlock.net/creating-a-source-generator-part-10-testing-your-incremental-generator-pipeline-outputs-are-cacheable/

public class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor NotInheritedFromDbContext = new(
        "CDG001",
        "Has an unsupported mapping method signature",
        $"The {nameof(UseDbContextAttribute)} used on class {{0}}, but class is not inherited from DbContext class, or if it is inherited from DbContext class add postfix 'DbContext' to a class name",
        "CRUD",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor WrongEntityGeneratorConfigurationSymbol = new(
        "CDG002",
        "Wrong entity generator configuration symbol",
        "Failed to read Entity Generator Configuration in {0}",
        "CRUD",
        DiagnosticSeverity.Error,
        true
    );
}