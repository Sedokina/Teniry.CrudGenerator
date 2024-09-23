using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;

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
            TemplatePath = Dto.TemplatePath.GetPath(built.GlobalConfiguration.TemplatesBasePath, built.OperationName),
            Name = Dto.NameConfigurationBuilder.GetName(entityScheme.EntityName, built.OperationName),
        };
        return built;
    }
}