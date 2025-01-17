using Teniry.CrudGenerator.Abstractions.DbContext;
using Microsoft.CodeAnalysis;

namespace Teniry.CrudGenerator.Diagnostics;

// Diagnostic models were taken from https://andrewlock.net/creating-a-source-generator-part-10-testing-your-incremental-generator-pipeline-outputs-are-cacheable/

public class DiagnosticDescriptors {
    public static readonly DiagnosticDescriptor NotInheritedFromDbContext = new(
        "CDG001",
        "Is not EntityFramework's DbContext class",
        $"The {nameof(UseDbContextAttribute)} used on class {{0}}, but class is not inherited from DbContext class",
        "CRUD",
        DiagnosticSeverity.Warning,
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

    public static readonly DiagnosticDescriptor DbContextNotFound = new(
        "CDG003",
        "Db context not found",
        $"There is no class with {nameof(UseDbContextAttribute)} attribute in the project",
        "CRUD",
        DiagnosticSeverity.Error,
        true
    );
}