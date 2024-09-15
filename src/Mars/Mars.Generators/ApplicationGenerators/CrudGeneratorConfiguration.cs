using Scriban;

namespace Mars.Generators.ApplicationGenerators;

public sealed class CrudGeneratorConfiguration
{
    public CrudGeneratorConfiguration()
    {
        InitDefault();
    }

    public string TemplatesBasePath { get; set; }
    public PutIntoNamespaceConfiguration PutIntoNamespaceBasePath { get; set; }
    public NameConfiguration FeatureNameConfiguration { get; set; }
    public BaseCommandGeneratorConfiguration CreateCommandCommandGenerator { get; set; }
    public BaseCommandGeneratorConfiguration DeleteCommandCommandGenerator { get; set; }
    public BaseCommandGeneratorConfiguration UpdateCommandCommandGenerator { get; set; }
    public BaseQueryGeneratorConfiguration GetByIdQueryGenerator { get; set; }
    public ListQueryGeneratorConfiguration GetListQueryGenerator { get; set; }

    // TODO: типизировать
    private void InitDefault()
    {
        TemplatesBasePath = "Mars.Generators.Templates.Crud";
        PutIntoNamespaceBasePath = new("{{assembly_name}}.Application.{{feature_name}}.{{function_name}}");
        FeatureNameConfiguration = new("{{entity_name}}Feature");
        CreateCommandCommandGenerator = new BaseCommandGeneratorConfiguration
        {
            FunctionNameConfiguration = new("Create{{entity_name}}"),
            CommandTemplatePath = $"{TemplatesBasePath}.Create.CreateCommand.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.Create.CreateHandler.txt",
            CommandNameConfiguration = new("Create{{entity_name}}Command"),
            HandlerNameConfiguration = new("Create{{entity_name}}Handler")
        };
        DeleteCommandCommandGenerator = new BaseCommandGeneratorConfiguration
        {
            FunctionNameConfiguration = new("Delete{{entity_name}}"),
            CommandTemplatePath = $"{TemplatesBasePath}.Delete.DeleteCommand.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.Delete.DeleteHandler.txt",
            CommandNameConfiguration = new("Delete{{entity_name}}Command"),
            HandlerNameConfiguration = new("Delete{{entity_name}}Handler")
        };
        UpdateCommandCommandGenerator = new BaseCommandGeneratorConfiguration
        {
            FunctionNameConfiguration = new("Update{{entity_name}}"),
            CommandTemplatePath = $"{TemplatesBasePath}.Update.UpdateCommand.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.Update.UpdateHandler.txt",
            CommandNameConfiguration = new("Update{{entity_name}}Command"),
            HandlerNameConfiguration = new("Update{{entity_name}}Handler")
        };
        GetByIdQueryGenerator = new BaseQueryGeneratorConfiguration
        {
            FunctionNameConfiguration = new("Get{{entity_name}}"),
            QueryTemplatePath = $"{TemplatesBasePath}.GetById.GetByIdQuery.txt",
            DtoTemplatePath = $"{TemplatesBasePath}.GetById.GetByIdDto.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.GetById.GetByIdHandler.txt",
            QueryNameConfiguration = new("Get{{entity_name}}Query"),
            DtoNameConfiguration = new("{{entity_name}}Dto"),
            HandlerNameConfiguration = new("Get{{entity_name}}Handler")
        };
        GetListQueryGenerator = new ListQueryGeneratorConfiguration
        {
            FunctionNameConfiguration = new("GetList{{entity_name}}"),
            QueryTemplatePath = $"{TemplatesBasePath}.GetList.GetListQuery.txt",
            DtoTemplatePath = $"{TemplatesBasePath}.GetList.GetListDto.txt",
            DtoListItemTemplatePath = $"{TemplatesBasePath}.GetList.GetListItemDto.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.GetList.GetListHandler.txt",
            QueryNameConfiguration = new("Get{{entity_name}}ListQuery"),
            DtoNameConfiguration = new("{{entity_name}}ListDto"),
            ListItemDtoNameConfiguration = new("{{entity_name}}ListItemDto"),
            HandlerNameConfiguration = new("Get{{entity_name}}ListHandler")
        };
    }
}

/// <summary>
/// Available string keys in namespace path:<br/>
///  - {{assembly_name}} <br/>
///  - {{feature_name}}<br/>
///  - {{function_name}}<br/>
/// </summary>
public class PutIntoNamespaceConfiguration(string namespacePath)
{
    public string GetNamespacePath(
        string entityName,
        string assemblyName,
        NameConfiguration featureName,
        NameConfiguration functionNameConfiguration)
    {
        var putIntoNamespaceTemplate = Template.Parse(namespacePath);
        return putIntoNamespaceTemplate.Render(new
        {
            AssemblyName = assemblyName,
            FeatureName = featureName.GetName(entityName),
            FunctionName = functionNameConfiguration.GetName(entityName)
        });
    }
}

/// <summary>
/// Available string in name:
///  - {{entity_name}}<br/>
/// </summary>
public class NameConfiguration(string name)
{
    public string GetName(string entityName)
    {
        var putIntoNamespaceTemplate = Template.Parse(name);
        return putIntoNamespaceTemplate.Render(new { entityName });
    }
}

public class BaseCommandGeneratorConfiguration
{
    public NameConfiguration FunctionNameConfiguration { get; set; }
    public string CommandTemplatePath { get; set; }
    public string HandlerTemplatePath { get; set; }
    public NameConfiguration CommandNameConfiguration { get; set; }
    public NameConfiguration HandlerNameConfiguration { get; set; }
}

public class BaseQueryGeneratorConfiguration
{
    public NameConfiguration FunctionNameConfiguration { get; set; }
    public string QueryTemplatePath { get; set; }
    public string DtoTemplatePath { get; set; }
    public string HandlerTemplatePath { get; set; }
    public NameConfiguration QueryNameConfiguration { get; set; }
    public NameConfiguration DtoNameConfiguration { get; set; }
    public NameConfiguration HandlerNameConfiguration { get; set; }
}

public class ListQueryGeneratorConfiguration
{
    public NameConfiguration FunctionNameConfiguration { get; set; }
    public string QueryTemplatePath { get; set; }
    public string DtoTemplatePath { get; set; }
    public string DtoListItemTemplatePath { get; set; }
    public string HandlerTemplatePath { get; set; }
    public NameConfiguration QueryNameConfiguration { get; set; }
    public NameConfiguration DtoNameConfiguration { get; set; }
    public NameConfiguration ListItemDtoNameConfiguration { get; set; }
    public NameConfiguration HandlerNameConfiguration { get; set; }
}