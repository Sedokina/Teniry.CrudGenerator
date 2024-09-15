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
            FunctionName = new("Create{{entity_name}}"),
            CommandTemplatePath = $"{TemplatesBasePath}.Create.CreateCommand.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.Create.CreateHandler.txt",
            CommandNameFormat = new("Create{{entity_name}}Command"),
            HandlerNameFormat = new("Create{{entity_name}}Handler")
        };
        DeleteCommandCommandGenerator = new BaseCommandGeneratorConfiguration
        {
            FunctionName = new("Delete{{entity_name}}"),
            CommandTemplatePath = $"{TemplatesBasePath}.Delete.DeleteCommand.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.Delete.DeleteHandler.txt",
            CommandNameFormat = new("Delete{{entity_name}}Command"),
            HandlerNameFormat = new("Delete{{entity_name}}Handler")
        };
        UpdateCommandCommandGenerator = new BaseCommandGeneratorConfiguration
        {
            FunctionName = new("Update{{entity_name}}"),
            CommandTemplatePath = $"{TemplatesBasePath}.Update.UpdateCommand.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.Update.UpdateHandler.txt",
            CommandNameFormat = new("Update{{entity_name}}Command"),
            HandlerNameFormat = new("Update{{entity_name}}Handler")
        };
        GetByIdQueryGenerator = new BaseQueryGeneratorConfiguration
        {
            FunctionNameFormat = new("Get{{entity_name}}"),
            QueryTemplatePath = $"{TemplatesBasePath}.GetById.GetByIdQuery.txt",
            DtoTemplatePath = $"{TemplatesBasePath}.GetById.GetByIdDto.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.GetById.GetByIdHandler.txt",
            QueryNameFormat = new("Get{{entity_name}}Query"),
            DtoNameFormat = new("{{entity_name}}Dto"),
            HandlerNameFormat = new("Get{{entity_name}}Handler")
        };
        GetListQueryGenerator = new ListQueryGeneratorConfiguration
        {
            FunctionName = new("GetList{{entity_name}}"),
            QueryTemplatePath = $"{TemplatesBasePath}.GetList.GetListQuery.txt",
            DtoTemplatePath = $"{TemplatesBasePath}.GetList.GetListDto.txt",
            DtoListItemTemplatePath = $"{TemplatesBasePath}.GetList.GetListItemDto.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.GetList.GetListHandler.txt",
            QueryNameFormat = new("Get{{entity_name}}ListQuery"),
            DtoNameFormat = new("{{entity_name}}ListDto"),
            ListItemDtoNameFormat = new("{{entity_name}}ListItemDto"),
            HandlerNameFormat = new("Get{{entity_name}}ListHandler")
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
        NameConfiguration functionNameFormat)
    {
        var putIntoNamespaceTemplate = Template.Parse(namespacePath);
        return putIntoNamespaceTemplate.Render(new
        {
            AssemblyName = assemblyName,
            FeatureName = featureName.GetName(entityName),
            FunctionName = functionNameFormat.GetName(entityName)
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
    public NameConfiguration FunctionName { get; set; }
    public string CommandTemplatePath { get; set; }
    public string HandlerTemplatePath { get; set; }
    public NameConfiguration CommandNameFormat { get; set; }
    public NameConfiguration HandlerNameFormat { get; set; }
}

public class BaseQueryGeneratorConfiguration
{
    public NameConfiguration FunctionNameFormat { get; set; }
    public string QueryTemplatePath { get; set; }
    public string DtoTemplatePath { get; set; }
    public string HandlerTemplatePath { get; set; }
    public NameConfiguration QueryNameFormat { get; set; }
    public NameConfiguration DtoNameFormat { get; set; }
    public NameConfiguration HandlerNameFormat { get; set; }
}

public class ListQueryGeneratorConfiguration
{
    public NameConfiguration FunctionName { get; set; }
    public string QueryTemplatePath { get; set; }
    public string DtoTemplatePath { get; set; }
    public string DtoListItemTemplatePath { get; set; }
    public string HandlerTemplatePath { get; set; }
    public NameConfiguration QueryNameFormat { get; set; }
    public NameConfiguration DtoNameFormat { get; set; }
    public NameConfiguration ListItemDtoNameFormat { get; set; }
    public NameConfiguration HandlerNameFormat { get; set; }
}