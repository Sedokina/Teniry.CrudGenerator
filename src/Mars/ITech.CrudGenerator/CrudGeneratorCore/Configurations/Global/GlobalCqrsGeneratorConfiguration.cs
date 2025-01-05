namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;

public sealed class GlobalCqrsGeneratorConfiguration
{
    public string AutogeneratedFileText { get; set; } = null!;
    public bool NullableEnable { get; set; } = true;
}