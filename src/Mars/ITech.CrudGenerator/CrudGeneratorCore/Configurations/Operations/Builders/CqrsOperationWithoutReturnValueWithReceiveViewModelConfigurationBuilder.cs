using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;

internal class CqrsOperationWithoutReturnValueWithReceiveViewModelConfigurationBuilder
    : CqrsOperationWithoutReturnValueConfigurationBuilder
{
    public NameConfigurationBuilder ViewModel { get; set; } = null!;

    public new CqrsOperationWithReturnValueWithReceiveViewModelGeneratorConfiguration Build(EntityScheme entityScheme)
    {
        var built = new CqrsOperationWithReturnValueWithReceiveViewModelGeneratorConfiguration();
        Init(built, entityScheme);
        built.ViewModel = new()
        {
            Name = ViewModel.GetName(entityScheme.EntityName, built.OperationName),
        };
        return built;
    }
}