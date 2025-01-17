using Microsoft.CodeAnalysis;

namespace Teniry.CrudGenerator.Diagnostics;

internal sealed record DiagnosticInfo {
    public DiagnosticDescriptor Descriptor { get; }
    public LocationInfo? Location { get; }

    public DiagnosticInfo(DiagnosticDescriptor descriptor, Location? location) {
        Descriptor = descriptor;
        Location = location is not null ? LocationInfo.CreateFrom(location) : null;
    }
}