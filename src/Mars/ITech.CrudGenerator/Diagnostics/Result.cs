using ITech.CrudGenerator.Core.Schemes;

namespace ITech.CrudGenerator.Diagnostics;

internal sealed record Result<TValue>(TValue Value, EquatableList<DiagnosticInfo> Errors)
{
    public TValue Value { get; } = Value;
    public EquatableList<DiagnosticInfo> Errors { get; } = Errors;
}