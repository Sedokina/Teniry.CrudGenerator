using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations.TypedBuiltConfigurations;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;

internal class CqrsListOperationGeneratorConfiguration : CqrsOperationWithReturnValueGeneratorConfiguration
{
    public FileTemplateBasedOperationConfiguration Filter { get; set; } = null!;
    public FileTemplateBasedOperationConfiguration DtoListItem { get; set; } = null!;
}