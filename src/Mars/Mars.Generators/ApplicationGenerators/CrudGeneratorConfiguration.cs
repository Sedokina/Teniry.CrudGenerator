using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;
using Mars.Generators.ApplicationGenerators.Core.Extensions;
using Scriban;

namespace Mars.Generators.ApplicationGenerators;

public sealed class CrudGeneratorConfiguration
{
    public CrudGeneratorConfiguration()
    {
        InitDefault();
    }

    public string AutogeneratedFileText { get; set; } = null!;
    public bool NullableEnable { get; set; } = true;
    public string TemplatesBasePath { get; set; } = null!;
    public PutBusinessLogicIntoNamespaceConfiguration BusinessLogicNamespaceBasePath { get; set; } = null!;
    public PutEndpointsIntoNamespaceConfiguration EndpointsNamespaceBasePath { get; set; } = null!;
    public NameConfiguration FeatureNameConfiguration { get; set; } = null!;
    public CommandWithReturnTypeGeneratorConfiguration CreateCommandCommandGenerator { get; set; } = null!;
    public BaseCommandGeneratorConfiguration DeleteCommandCommandGenerator { get; set; } = null!;
    public BaseCommandGeneratorConfiguration UpdateCommandCommandGenerator { get; set; } = null!;
    public BaseQueryGeneratorConfiguration GetByIdQueryGenerator { get; set; } = null!;
    public ListQueryGeneratorConfiguration GetListQueryGenerator { get; set; } = null!;

    private void InitDefault()
    {
        AutogeneratedFileText = @"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a crud generator tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------";
        NullableEnable = true;
        TemplatesBasePath = "Mars.Generators.Templates.Crud";
        BusinessLogicNamespaceBasePath = new("{{assembly_name}}.Application.{{feature_name}}.{{function_name}}");
        EndpointsNamespaceBasePath = new("{{assembly_name}}.Endpoints.{{entity_name}}Endpoints");
        FeatureNameConfiguration = new("{{entity_name}}Feature");
        CreateCommandCommandGenerator = new CommandWithReturnTypeGeneratorConfiguration
        {
            FullConfiguration = this,
            FunctionNameConfiguration = new("Create{{entity_name}}"),
            CommandTemplatePath = $"{TemplatesBasePath}.Create.CreateCommand.txt",
            DtoTemplatePath = $"{TemplatesBasePath}.Create.CreatedDto.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.Create.CreateHandler.txt",
            CommandNameConfiguration = new("Create{{entity_name}}Command"),
            DtoNameConfiguration = new NameConfiguration("Created{{entity_name}}Dto"),
            HandlerNameConfiguration = new("Create{{entity_name}}Handler"),
            EndpointTemplatePath = $"{TemplatesBasePath}.Create.CreateEndpoint.txt",
            EndpointNameConfiguration = new("Create{{entity_name}}Endpoint"),
            EndpointRouteConfiguration = new("/{{entity_name}}/create", "CreateAsync")
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
            EndpointNameConfiguration = new("Delete{{entity_name}}Endpoint"),
            EndpointRouteConfiguration = new("/{{entity_name}}/{{id_param_name}}/delete", "DeleteAsync")
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
            EndpointNameConfiguration = new("Update{{entity_name}}Endpoint"),
            EndpointRouteConfiguration = new("/{{entity_name}}/{{id_param_name}}/update", "UpdateAsync")
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
            EndpointNameConfiguration = new("Get{{entity_name}}Endpoint"),
            EndpointRouteConfiguration = new("/{{entity_name}}/{{id_param_name}}", "GetAsync")
        };
        GetListQueryGenerator = new ListQueryGeneratorConfiguration
        {
            FullConfiguration = this,
            FunctionNameConfiguration = new("GetList{{entity_name}}"),
            QueryTemplatePath = $"{TemplatesBasePath}.GetList.GetListQuery.txt",
            DtoTemplatePath = $"{TemplatesBasePath}.GetList.GetListDto.txt",
            DtoListItemTemplatePath = $"{TemplatesBasePath}.GetList.GetListItemDto.txt",
            FilterTemplatePath = $"{TemplatesBasePath}.GetList.GetListFilter.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.GetList.GetListHandler.txt",
            QueryNameConfiguration = new("Get{{plural_entity_name}}Query"),
            DtoNameConfiguration = new("{{plural_entity_name}}Dto"),
            ListItemDtoNameConfiguration = new("{{plural_entity_name}}ListItemDto"),
            FilterNameConfiguration = new("Get{{plural_entity_name}}Filter"),
            HandlerNameConfiguration = new("Get{{plural_entity_name}}Handler"),
            EndpointTemplatePath = $"{TemplatesBasePath}.GetList.GetListEndpoint.txt",
            EndpointNameConfiguration = new("Get{{plural_entity_name}}Endpoint"),
            EndpointRouteConfiguration = new("/{{entity_name}}", "GetAsync")
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
        EntityName entityName,
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
        EntityName entityName,
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
    public string GetName(EntityName entityName)
    {
        var putIntoNamespaceTemplate = Template.Parse(name);
        var model = new
        {
            EntityName = entityName.Name,
            PluralEntityName = entityName.PluralName
        };
        return putIntoNamespaceTemplate.Render(model);
    }
}

/// <summary>
/// Available string in name:
///  - {{entity_name}}<br/>
///  - {{id_param_name}}<br/>
/// </summary>
public class EndpointRouteConfiguration(string name, string functionName)
{
    public string FunctionName { get; } = functionName;

    public string GetRoute(string entityName, List<string>? idParams = null)
    {
        var putIntoNamespaceTemplate = Template.Parse(name);
        entityName = entityName.ToLower();

        if (idParams == null)
        {
            return putIntoNamespaceTemplate.Render(new { entityName });
        }

        var idParamName = string.Join("/", idParams.Select(x => $"{{{x}}}"));
        return putIntoNamespaceTemplate.Render(new { entityName, idParamName });
    }
}

public interface IQueryCommandGeneratorConfiguration
{
    public CrudGeneratorConfiguration FullConfiguration { get; set; }
    public NameConfiguration FunctionNameConfiguration { get; set; }
}

public class BaseCommandGeneratorConfiguration : IQueryCommandGeneratorConfiguration
{
    public CrudGeneratorConfiguration FullConfiguration { get; set; } = null!;
    public NameConfiguration FunctionNameConfiguration { get; set; } = null!;
    public string CommandTemplatePath { get; set; } = null!;
    public string HandlerTemplatePath { get; set; } = null!;
    public string EndpointTemplatePath { get; set; } = null!;
    public NameConfiguration CommandNameConfiguration { get; set; } = null!;
    public NameConfiguration HandlerNameConfiguration { get; set; } = null!;
    public NameConfiguration EndpointNameConfiguration { get; set; } = null!;
    public EndpointRouteConfiguration EndpointRouteConfiguration { get; set; } = null!;
}

public class CommandWithReturnTypeGeneratorConfiguration : IQueryCommandGeneratorConfiguration
{
    public CrudGeneratorConfiguration FullConfiguration { get; set; } = null!;
    public NameConfiguration FunctionNameConfiguration { get; set; } = null!;
    public string CommandTemplatePath { get; set; } = null!;
    public string DtoTemplatePath { get; set; } = null!;
    public string HandlerTemplatePath { get; set; } = null!;
    public string EndpointTemplatePath { get; set; } = null!;
    public NameConfiguration CommandNameConfiguration { get; set; } = null!;
    public NameConfiguration DtoNameConfiguration { get; set; } = null!;
    public NameConfiguration HandlerNameConfiguration { get; set; } = null!;
    public NameConfiguration EndpointNameConfiguration { get; set; } = null!;
    public EndpointRouteConfiguration EndpointRouteConfiguration { get; set; } = null!;
}

public class BaseQueryGeneratorConfiguration : IQueryCommandGeneratorConfiguration
{
    public CrudGeneratorConfiguration FullConfiguration { get; set; } = null!;
    public NameConfiguration FunctionNameConfiguration { get; set; } = null!;
    public string QueryTemplatePath { get; set; } = null!;
    public string DtoTemplatePath { get; set; } = null!;
    public string HandlerTemplatePath { get; set; } = null!;
    public string EndpointTemplatePath { get; set; } = null!;
    public NameConfiguration QueryNameConfiguration { get; set; } = null!;
    public NameConfiguration DtoNameConfiguration { get; set; } = null!;
    public NameConfiguration HandlerNameConfiguration { get; set; } = null!;
    public NameConfiguration EndpointNameConfiguration { get; set; } = null!;
    public EndpointRouteConfiguration EndpointRouteConfiguration { get; set; } = null!;
}

public class ListQueryGeneratorConfiguration : IQueryCommandGeneratorConfiguration
{
    public CrudGeneratorConfiguration FullConfiguration { get; set; } = null!;
    public NameConfiguration FunctionNameConfiguration { get; set; } = null!;
    public string QueryTemplatePath { get; set; } = null!;
    public string DtoTemplatePath { get; set; } = null!;
    public string DtoListItemTemplatePath { get; set; } = null!;
    public string FilterTemplatePath { get; set; } = null!;
    public string HandlerTemplatePath { get; set; } = null!;
    public string EndpointTemplatePath { get; set; } = null!;
    public NameConfiguration QueryNameConfiguration { get; set; } = null!;
    public NameConfiguration DtoNameConfiguration { get; set; } = null!;
    public NameConfiguration ListItemDtoNameConfiguration { get; set; } = null!;
    public NameConfiguration FilterNameConfiguration { get; set; } = null!;
    public NameConfiguration HandlerNameConfiguration { get; set; } = null!;
    public NameConfiguration EndpointNameConfiguration { get; set; } = null!;
    public EndpointRouteConfiguration EndpointRouteConfiguration { get; set; } = null!;
}