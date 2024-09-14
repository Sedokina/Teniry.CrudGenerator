namespace Mars.Generators.ApplicationGenerators;

public sealed class CrudGeneratorConfiguration
{
    private static CrudGeneratorConfiguration _instance;

    public CrudGeneratorConfiguration()
    {
        InitDefault();
    }

    public static CrudGeneratorConfiguration Instance => _instance ??= new CrudGeneratorConfiguration();

    public string TemplatesBasePath { get; set; }
    public string PutIntoNamespaceBasePath { get; set; }
    public BaseCommandGeneratorConfiguration CreateCommandCommandGenerator { get; set; }
    public BaseCommandGeneratorConfiguration DeleteCommandCommandGenerator { get; set; }
    public BaseCommandGeneratorConfiguration UpdateCommandCommandGenerator { get; set; }
    public BaseQueryGeneratorConfiguration GetByIdQueryGenerator { get; set; }
    public ListQueryGeneratorConfiguration GetListQueryGenerator { get; set; }

    private void InitDefault()
    {
        TemplatesBasePath = "Mars.Generators.Templates.Crud";
        PutIntoNamespaceBasePath = "{{assembly_name}}.Application.{{feature_name}}";
        CreateCommandCommandGenerator = new BaseCommandGeneratorConfiguration
        {
            CommandTemplatePath = $"{TemplatesBasePath}.Create.CreateCommand.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.Create.CreateHandler.txt",
            CommandNameFormat = "Create{0}Command",
            HandlerNameFormat = "Create{0}Handler"
        };
        DeleteCommandCommandGenerator = new BaseCommandGeneratorConfiguration
        {
            CommandTemplatePath = $"{TemplatesBasePath}.Delete.DeleteCommand.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.Delete.DeleteHandler.txt",
            CommandNameFormat = "Delete{0}Command",
            HandlerNameFormat = "Delete{0}Handler"
        };
        UpdateCommandCommandGenerator = new BaseCommandGeneratorConfiguration
        {
            CommandTemplatePath = $"{TemplatesBasePath}.Update.UpdateCommand.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.Update.UpdateHandler.txt",
            CommandNameFormat = "Update{0}Command",
            HandlerNameFormat = "Update{0}Handler"
        };
        GetByIdQueryGenerator = new BaseQueryGeneratorConfiguration
        {
            QueryTemplatePath = $"{TemplatesBasePath}.GetById.GetByIdQuery.txt",
            DtoTemplatePath = $"{TemplatesBasePath}.GetById.GetByIdDto.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.GetById.GetByIdHandler.txt",
            QueryNameFormat = "Get{0}Query",
            DtoNameFormat = "{0}Dto",
            HandlerNameFormat = "Get{0}Handler"
        };
        GetListQueryGenerator = new ListQueryGeneratorConfiguration
        {
            QueryTemplatePath = $"{TemplatesBasePath}.GetList.GetListQuery.txt",
            DtoTemplatePath = $"{TemplatesBasePath}.GetList.GetListDto.txt",
            DtoListItemTemplatePath = $"{TemplatesBasePath}.GetList.GetListItemDto.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.GetList.GetListHandler.txt",
            QueryNameFormat = "Get{0}ListQuery",
            DtoNameFormat = "{0}ListDto",
            ListItemDtoNameFormat = "{0}ListItemDto",
            HandlerNameFormat = "Get{0}ListHandler"
        };
    }
}

public class BaseCommandGeneratorConfiguration
{
    public string CommandTemplatePath { get; set; }
    public string HandlerTemplatePath { get; set; }
    public string CommandNameFormat { get; set; }
    public string HandlerNameFormat { get; set; }

    public string GetCommandName(string entityName)
    {
        return string.Format(CommandNameFormat, entityName);
    }

    public string GetHandlerName(string entityName)
    {
        return string.Format(HandlerNameFormat, entityName);
    }
}

public class BaseQueryGeneratorConfiguration
{
    public string QueryTemplatePath { get; set; }
    public string DtoTemplatePath { get; set; }
    public string HandlerTemplatePath { get; set; }
    public string QueryNameFormat { get; set; }
    public string DtoNameFormat { get; set; }
    public string HandlerNameFormat { get; set; }

    public string GetQueryName(string entityName)
    {
        return string.Format(QueryNameFormat, entityName);
    }

    public string GetDtoName(string entityName)
    {
        return string.Format(DtoNameFormat, entityName);
    }

    public string GetHandlerName(string entityName)
    {
        return string.Format(HandlerNameFormat, entityName);
    }
}

public class ListQueryGeneratorConfiguration
{
    public string QueryTemplatePath { get; set; }
    public string DtoTemplatePath { get; set; }
    public string DtoListItemTemplatePath { get; set; }
    public string HandlerTemplatePath { get; set; }
    public string QueryNameFormat { get; set; }
    public string DtoNameFormat { get; set; }
    public string ListItemDtoNameFormat { get; set; }
    public string HandlerNameFormat { get; set; }

    public string GetQueryName(string entityName)
    {
        return string.Format(QueryNameFormat, entityName);
    }

    public string GetDtoName(string entityName)
    {
        return string.Format(DtoNameFormat, entityName);
    }

    public string GetListItemDtoName(string entityName)
    {
        return string.Format(ListItemDtoNameFormat, entityName);
    }

    public string GetHandlerName(string entityName)
    {
        return string.Format(HandlerNameFormat, entityName);
    }
}