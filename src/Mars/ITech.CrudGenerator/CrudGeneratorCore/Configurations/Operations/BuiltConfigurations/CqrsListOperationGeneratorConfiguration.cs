namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;

internal class CqrsListOperationGeneratorConfiguration : CqrsOperationWithReturnValueGeneratorConfiguration
{
    public string Filter { get; set; } = null!;
    public string DtoListItem { get; set; } = null!;
}