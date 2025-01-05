using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;

internal class CqrsListOperationConfigurationBuilder : CqrsOperationWithReturnValueConfigurationBuilder
{
    public FileTemplateBasedOperationConfigurationBuilder Filter { get; set; } = null!;
    public FileTemplateBasedOperationConfigurationBuilder DtoListItem { get; set; } = null!;

    public new CqrsListOperationGeneratorConfiguration Build(EntityScheme entityScheme)
    {
        var built = new CqrsListOperationGeneratorConfiguration();
        Init(built, entityScheme);
        built.Dto = new()
        {
            Name = Dto.NameConfigurationBuilder.GetName(entityScheme.EntityName, built.OperationName),
        };

        built.Filter = new()
        {
            Name = Filter.NameConfigurationBuilder.GetName(entityScheme.EntityName, built.OperationName),
        };

        built.DtoListItem = new()
        {
            Name = DtoListItem.NameConfigurationBuilder.GetName(entityScheme.EntityName, built.OperationName),
        };

        return built;
    }
}