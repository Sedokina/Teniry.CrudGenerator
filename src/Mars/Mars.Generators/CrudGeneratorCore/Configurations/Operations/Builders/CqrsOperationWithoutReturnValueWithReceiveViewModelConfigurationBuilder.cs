using Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;
using Mars.Generators.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using Mars.Generators.CrudGeneratorCore.Schemes.Entity;

namespace Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders;

internal class CqrsOperationWithoutReturnValueWithReceiveViewModelConfigurationBuilder
    : CqrsOperationWithoutReturnValueConfigurationBuilder
{
    public FileTemplateBasedOperationConfigurationBuilder ViewModel { get; set; } = null!;

    public new CqrsOperationWithReturnValueWithReceiveViewModelGeneratorConfiguration Build(EntityScheme entityScheme)
    {
        var built = new CqrsOperationWithReturnValueWithReceiveViewModelGeneratorConfiguration();
        Init(built, entityScheme);
        built.ViewModel = new()
        {
            TemplatePath = ViewModel.TemplatePath
                .GetPath(built.GlobalConfiguration.TemplatesBasePath, built.OperationName),
            Name = ViewModel.NameConfigurationBuilder.GetName(entityScheme.EntityName, built.OperationName),
        };
        return built;
    }
}