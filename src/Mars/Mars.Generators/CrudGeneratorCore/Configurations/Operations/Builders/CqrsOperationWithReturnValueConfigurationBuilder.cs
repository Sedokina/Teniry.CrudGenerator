using Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;
using Mars.Generators.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using Mars.Generators.CrudGeneratorCore.Schemes.Entity;

namespace Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders;

internal class CqrsOperationWithReturnValueConfigurationBuilder :
    CqrsOperationWithoutReturnValueConfigurationBuilder
{
    public FileTemplateBasedOperationConfigurationBuilder Dto { get; set; } = null!;

    public new CqrsOperationWithReturnValueGeneratorConfiguration Build(EntityScheme entityScheme)
    {
        var built = new CqrsOperationWithReturnValueGeneratorConfiguration();
        Init(built, entityScheme);
        built.Dto = new()
        {
            TemplatePath = Dto.TemplatePath.GetPath(built.GlobalConfiguration.TemplatesBasePath),
            Name = Dto.NameConfigurationBuilder.GetName(entityScheme.EntityName),
        };
        return built;
    }
}