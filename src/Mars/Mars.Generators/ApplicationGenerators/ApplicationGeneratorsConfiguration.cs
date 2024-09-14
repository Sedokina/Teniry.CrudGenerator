namespace Mars.Generators.ApplicationGenerators;

public sealed class ApplicationGeneratorsConfiguration
{
    private static ApplicationGeneratorsConfiguration _instance = null;

    public static ApplicationGeneratorsConfiguration Instance => _instance ??= new ApplicationGeneratorsConfiguration();

    public string TemplatesBasePath { get; set; }
    public BaseCommandGeneratorConfiguration CreateCommandCommandGenerator { get; set; }
    public BaseCommandGeneratorConfiguration DeleteCommandCommandGenerator { get; set; }
    public BaseCommandGeneratorConfiguration UpdateCommandCommandGenerator { get; set; }
    public BaseQueryGeneratorConfiguration GetByIdQueryGenerator { get; set; }
    public ListQueryGeneratorConfiguration GetListQueryGenerator { get; set; }

    public ApplicationGeneratorsConfiguration()
    {
        InitDefault();
    }

    private void InitDefault()
    {
        TemplatesBasePath = "Mars.Generators.Templates";
        CreateCommandCommandGenerator = new BaseCommandGeneratorConfiguration
        {
            CommandTemplatePath = $"{TemplatesBasePath}.CreateCommand.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.CreateHandler.txt",
            CommandNameFormat = "Create{0}Command",
            HandlerNameFormat = "Create{0}Handler"
        };
        DeleteCommandCommandGenerator = new BaseCommandGeneratorConfiguration
        {
            CommandTemplatePath = $"{TemplatesBasePath}.DeleteCommand.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.DeleteHandler.txt",
            CommandNameFormat = "Delete{0}Command",
            HandlerNameFormat = "Delete{0}Handler"
        };
        UpdateCommandCommandGenerator = new BaseCommandGeneratorConfiguration
        {
            CommandTemplatePath = $"{TemplatesBasePath}.UpdateCommand.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.UpdateHandler.txt",
            CommandNameFormat = "Update{0}Command",
            HandlerNameFormat = "Update{0}Handler"
        };
        GetByIdQueryGenerator = new BaseQueryGeneratorConfiguration
        {
            QueryTemplatePath = $"{TemplatesBasePath}.GetByIdQuery.txt",
            DtoTemplatePath = $"{TemplatesBasePath}.GetByIdDto.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.GetByIdHandler.txt",
            QueryNameFormat = "Get{0}Query",
            DtoNameFormat = "{0}Dto",
            HandlerNameFormat = "Get{0}Handler"
        };
        GetListQueryGenerator = new ListQueryGeneratorConfiguration
        {
            QueryTemplatePath = $"{TemplatesBasePath}.GetListQuery.txt",
            DtoTemplatePath = $"{TemplatesBasePath}.GetListDto.txt",
            DtoListItemTemplatePath = $"{TemplatesBasePath}.GetListItemDto.txt",
            HandlerTemplatePath = $"{TemplatesBasePath}.GetListHandler.txt",
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
    public string GetCommandName(string entityName) => string.Format(CommandNameFormat, entityName);
    public string GetHandlerName(string entityName) => string.Format(HandlerNameFormat, entityName);
}

public class BaseQueryGeneratorConfiguration
{
    public string QueryTemplatePath { get; set; }
    public string DtoTemplatePath { get; set; }
    public string HandlerTemplatePath { get; set; }
    public string QueryNameFormat { get; set; }
    public string DtoNameFormat { get; set; }
    public string HandlerNameFormat { get; set; }
    public string GetQueryName(string entityName) => string.Format(QueryNameFormat, entityName);
    public string GetDtoName(string entityName) => string.Format(DtoNameFormat, entityName);
    public string GetHandlerName(string entityName) => string.Format(HandlerNameFormat, entityName);
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
    public string GetQueryName(string entityName) => string.Format(QueryNameFormat, entityName);
    public string GetDtoName(string entityName) => string.Format(DtoNameFormat, entityName);
    public string GetListItemDtoName(string entityName) => string.Format(ListItemDtoNameFormat, entityName);
    public string GetHandlerName(string entityName) => string.Format(HandlerNameFormat, entityName);
}