using Teniry.CrudGenerator.Core.Schemes;

namespace Teniry.CrudGenerator.Diagnostics;

internal sealed record Result<TValue>(TValue Value, EquatableList<DiagnosticInfo> Diagnostics) {
    public TValue Value { get; } = Value;
    public EquatableList<DiagnosticInfo> Diagnostics { get; } = Diagnostics;
}