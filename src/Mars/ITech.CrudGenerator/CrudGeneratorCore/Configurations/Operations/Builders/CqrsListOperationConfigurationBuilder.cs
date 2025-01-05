using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;

internal class CqrsListOperationConfigurationBuilder : CqrsOperationWithReturnValueConfigurationBuilder
{
    public NameConfigurationBuilder Filter { get; set; } = null!;
    public NameConfigurationBuilder DtoListItem { get; set; } = null!;

    public new CqrsListOperationGeneratorConfiguration Build(EntityScheme entityScheme)
    {
        var built = new CqrsListOperationGeneratorConfiguration();
        Init(built, entityScheme);
        built.Dto = Dto.GetName(entityScheme.EntityName, built.OperationName);
        built.Filter = Filter.GetName(entityScheme.EntityName, built.OperationName);
        built.DtoListItem = DtoListItem.GetName(entityScheme.EntityName, built.OperationName);

        return built;
    }
}