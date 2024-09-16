using Scriban;

namespace Mars.Generators.ApplicationGenerators;

public sealed class CrudGeneratorConfiguration
{
    public CrudGeneratorConfiguration()
    {
        InitDefault();
    }

    public string TemplatesBasePath { get; set; }
    public PutBusinessLogicIntoNamespaceConfiguration BusinessLogicNamespaceBasePath { get; set; }
    public PutEndpointsIntoNamespaceConfiguration EndpointsNamespaceBasePath { get; set; }
    public NameConfiguration FeatureNameConfiguration { get; set; }
    public BaseCommandGeneratorConfiguration CreateCommandCommandGenerator { get; set; }
    public BaseCommandGeneratorConfiguration DeleteCommandCommandGenerator { get; set; }
    public BaseCommandGeneratorConfiguration UpdateCommandCommandGenerator { get; set; }
    public BaseQueryGeneratorConfiguration GetByIdQueryGenerator { get; set; }
    public ListQueryGeneratorConfiguration GetListQueryGenerator { get; set; }

    private void InitDefault()
    {
        TemplatesBasePath = "Mars.Generators.Templates.Crud";
        BusinessLogicNamespaceBasePath = new("{{assembly_name}}.Application.{{feature_name}}.{{function_name}}");
        EndpointsNamespaceBasePath = new("{{assembly_name}}.Endpoints.{{entity_name}}Endpoints");
        FeatureNameConfiguration = new("{{entity_name}}Feature");
        CreateCommandCommandGenerator = new BaseCommandGeneratorConfiguration
        {
            FullConfiguration = this,
            FunctionNameConfiguration = new("Create{{entity_name}}"),
            CommandTemplatePath = $"{TemplatesBasePath}.Create.CreateCommand.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.Create.CreateHandler.txt",
            CommandNameConfiguration = new("Create{{entity_name}}Command"),
            HandlerNameConfiguration = new("Create{{entity_name}}Handler"),
            EndpointTemplatePath = $"{TemplatesBasePath}.Create.CreateEndpoint.txt",
            EndpointNameConfiguration = new("Create{{entity_name}}Endpoint")
        };
        DeleteCommandCommandGenerator = new BaseCommandGeneratorConfiguration
        {
            FullConfiguration = this,
            FunctionNameConfiguration = new("Delete{{entity_name}}"),
            CommandTemplatePath = $"{TemplatesBasePath}.Delete.DeleteCommand.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.Delete.DeleteHandler.txt",
            CommandNameConfiguration = new("Delete{{entity_name}}Command"),
            HandlerNameConfiguration = new("Delete{{entity_name}}Handler"),
            EndpointTemplatePath = $"{TemplatesBasePath}.Delete.DeleteEndpoint.txt",
            EndpointNameConfiguration = new("Delete{{entity_name}}Endpoint")
        };
        UpdateCommandCommandGenerator = new BaseCommandGeneratorConfiguration
        {
            FullConfiguration = this,
            FunctionNameConfiguration = new("Update{{entity_name}}"),
            CommandTemplatePath = $"{TemplatesBasePath}.Update.UpdateCommand.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.Update.UpdateHandler.txt",
            CommandNameConfiguration = new("Update{{entity_name}}Command"),
            HandlerNameConfiguration = new("Update{{entity_name}}Handler"),
            EndpointTemplatePath = $"{TemplatesBasePath}.Update.UpdateEndpoint.txt",
            EndpointNameConfiguration = new("Update{{entity_name}}Endpoint")
        };
        GetByIdQueryGenerator = new BaseQueryGeneratorConfiguration
        {
            FullConfiguration = this,
            FunctionNameConfiguration = new("Get{{entity_name}}"),
            QueryTemplatePath = $"{TemplatesBasePath}.GetById.GetByIdQuery.txt",
            DtoTemplatePath = $"{TemplatesBasePath}.GetById.GetByIdDto.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.GetById.GetByIdHandler.txt",
            QueryNameConfiguration = new("Get{{entity_name}}Query"),
            DtoNameConfiguration = new("{{entity_name}}Dto"),
            HandlerNameConfiguration = new("Get{{entity_name}}Handler"),
            EndpointTemplatePath = $"{TemplatesBasePath}.GetById.GetByIdEndpoint.txt",
            EndpointNameConfiguration = new("GetById{{entity_name}}Endpoint")
        };
        GetListQueryGenerator = new ListQueryGeneratorConfiguration
        {
            FullConfiguration = this,
            FunctionNameConfiguration = new("GetList{{entity_name}}"),
            QueryTemplatePath = $"{TemplatesBasePath}.GetList.GetListQuery.txt",
            DtoTemplatePath = $"{TemplatesBasePath}.GetList.GetListDto.txt",
            DtoListItemTemplatePath = $"{TemplatesBasePath}.GetList.GetListItemDto.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.GetList.GetListHandler.txt",
            QueryNameConfiguration = new("Get{{entity_name}}ListQuery"),
            DtoNameConfiguration = new("{{entity_name}}ListDto"),
            ListItemDtoNameConfiguration = new("{{entity_name}}ListItemDto"),
            HandlerNameConfiguration = new("Get{{entity_name}}ListHandler"),
            EndpointTemplatePath = $"{TemplatesBasePath}.GetList.GetListEndpoint.txt",
            EndpointNameConfiguration = new("Get{{entity_name}}ListEndpoint")
        };
    }
}

/// <summary>
/// Available string keys in namespace path:<br/>
///  - {{assembly_name}} <br/>
///  - {{feature_name}}<br/>
///  - {{function_name}}<br/>
/// </summary>
public class PutBusinessLogicIntoNamespaceConfiguration(string namespacePath)
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
/// Available string keys in namespace path:<br/>
///  - {{assembly_name}} <br/>
///  - {{feature_name}}<br/>
///  - {{function_name}}<br/>
/// </summary>
public class PutEndpointsIntoNamespaceConfiguration(string namespacePath)
{
    public string GetNamespacePath(
        string entityName,
        string assemblyName)
    {
        var putIntoNamespaceTemplate = Template.Parse(namespacePath);
        return putIntoNamespaceTemplate.Render(new
        {
            AssemblyName = assemblyName,
            EntityName = entityName
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

public interface IQueryCommandGeneratorConfiguration
{
    public CrudGeneratorConfiguration FullConfiguration { get; set; }
    public NameConfiguration FunctionNameConfiguration { get; set; }
}

public class BaseCommandGeneratorConfiguration : IQueryCommandGeneratorConfiguration
{
    public CrudGeneratorConfiguration FullConfiguration { get; set; }
    public NameConfiguration FunctionNameConfiguration { get; set; }
    public string CommandTemplatePath { get; set; }
    public string HandlerTemplatePath { get; set; }
    public string EndpointTemplatePath { get; set; }
    public NameConfiguration CommandNameConfiguration { get; set; }
    public NameConfiguration HandlerNameConfiguration { get; set; }
    public NameConfiguration EndpointNameConfiguration { get; set; }
}

public class BaseQueryGeneratorConfiguration : IQueryCommandGeneratorConfiguration
{
    public CrudGeneratorConfiguration FullConfiguration { get; set; }
    public NameConfiguration FunctionNameConfiguration { get; set; }
    public string QueryTemplatePath { get; set; }
    public string DtoTemplatePath { get; set; }
    public string HandlerTemplatePath { get; set; }
    public string EndpointTemplatePath { get; set; }
    public NameConfiguration QueryNameConfiguration { get; set; }
    public NameConfiguration DtoNameConfiguration { get; set; }
    public NameConfiguration HandlerNameConfiguration { get; set; }
    public NameConfiguration EndpointNameConfiguration { get; set; }
}

public class ListQueryGeneratorConfiguration : IQueryCommandGeneratorConfiguration
{
    public CrudGeneratorConfiguration FullConfiguration { get; set; }
    public NameConfiguration FunctionNameConfiguration { get; set; }
    public string QueryTemplatePath { get; set; }
    public string DtoTemplatePath { get; set; }
    public string DtoListItemTemplatePath { get; set; }
    public string HandlerTemplatePath { get; set; }
    public string EndpointTemplatePath { get; set; }
    public NameConfiguration QueryNameConfiguration { get; set; }
    public NameConfiguration DtoNameConfiguration { get; set; }
    public NameConfiguration ListItemDtoNameConfiguration { get; set; }
    public NameConfiguration HandlerNameConfiguration { get; set; }
    public NameConfiguration EndpointNameConfiguration { get; set; }
}