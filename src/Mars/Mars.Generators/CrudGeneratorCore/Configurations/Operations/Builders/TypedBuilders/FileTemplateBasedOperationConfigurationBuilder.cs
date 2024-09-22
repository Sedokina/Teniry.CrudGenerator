namespace Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;

internal class FileTemplateBasedOperationConfigurationBuilder
{
    public string TemplatePath { get; set; } = null!;
    public NameConfigurationBuilder NameConfigurationBuilder { get; set; } = null!;
}