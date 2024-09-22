using Scriban;

namespace Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;

/// <summary>
///     Available string in name:
///     - {{templates_base_path}}
/// </summary>
internal class FileTemplatePathConfigurationBuilder(string path)
{
    public string GetPath(string templatesBasePath)
    {
        var template = Template.Parse(path);
        var model = new
        {
            TemplatesBasePath = templatesBasePath
        };
        return template.Render(model);
    }
}