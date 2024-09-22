using Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;
using Mars.Generators.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using Mars.Generators.CrudGeneratorCore.Schemes.Entity;

namespace Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders;

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
            TemplatePath = Dto.TemplatePath.GetPath(built.GlobalConfiguration.TemplatesBasePath, built.OperationName),
            Name = Dto.NameConfigurationBuilder.GetName(entityScheme.EntityName, built.OperationName),
        };

        built.Filter = new()
        {
            TemplatePath =
                Filter.TemplatePath.GetPath(built.GlobalConfiguration.TemplatesBasePath, built.OperationName),
            Name = Filter.NameConfigurationBuilder.GetName(entityScheme.EntityName, built.OperationName),
        };

        built.DtoListItem = new()
        {
            TemplatePath =
                DtoListItem.TemplatePath.GetPath(built.GlobalConfiguration.TemplatesBasePath, built.OperationName),
            Name = DtoListItem.NameConfigurationBuilder.GetName(entityScheme.EntityName, built.OperationName),
        };

        return built;
    }
}