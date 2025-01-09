namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;

public sealed record GlobalCqrsGeneratorConfiguration
{
    public string AutogeneratedFileText { get; set; } = null!;
    public bool NullableEnable { get; set; } = true;
}