using Scriban;

namespace Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;

/// <summary>
///     Available string in name:
///     - {{templates_base_path}} <br/>
///     - {{operation_name}}
/// </summary>
internal class FileTemplatePathConfigurationBuilder(string path)
{
    public string GetPath(string templatesBasePath, string operationName)
    {
        var template = Template.Parse(path);
        var model = new
        {
            TemplatesBasePath = templatesBasePath,
            OperationName = operationName
        };
        return template.Render(model);
    }
}