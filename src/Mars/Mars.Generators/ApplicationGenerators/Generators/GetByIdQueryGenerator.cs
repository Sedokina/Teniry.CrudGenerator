using Mars.Generators.ApplicationGenerators.Core;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Generators;

public class GetByIdQueryGenerator : BaseGenerator<BaseQueryGeneratorConfiguration>
{
    private readonly string _dtoName;
    private readonly string _handlerName;
    private readonly string _queryName;

    public GetByIdQueryGenerator(
        GeneratorExecutionContext context,
        ISymbol symbol,
        BaseQueryGeneratorConfiguration configuration) : base(context, symbol, configuration)
    {
        _queryName = Configuration.QueryNameConfiguration.GetName(_entityName);
        _dtoName = Configuration.DtoNameConfiguration.GetName(_entityName);
        _handlerName = Configuration.HandlerNameConfiguration.GetName(_entityName);
    }

    public void RunGenerator()
    {
        GenerateQuery(Configuration.QueryTemplatePath);
        GenerateDto(Configuration.DtoTemplatePath);
        GenerateHandler(Configuration.HandlerTemplatePath);
    }

    private void GenerateQuery(string templatePath)
    {
        var properties = PropertiesExtractor.GetPrimaryKeysOfEntityAsProperties(Symbol);
        var model = new
        {
            QueryName = _queryName,
            Properties = properties
        };

        WriteFile(templatePath, model, _queryName);
    }

    private void GenerateDto(string templatePath)
    {
        var properties = PropertiesExtractor.GetAllPropertiesOfEntity(Symbol);
        var model = new
        {
            DtoName = _dtoName,
            Properties = properties
        };

        WriteFile(templatePath, model, _dtoName);
    }

    private void GenerateHandler(string templatePath)
    {
        var properties = PropertiesExtractor.GetPrimaryKeyNamesOfEntity(Symbol, "query");
        var model = new
        {
            QueryName = _queryName,
            HandlerName = _handlerName,
            DtoName = _dtoName,
            FindProperties = string.Join(", ", properties)
        };

        WriteFile(templatePath, model, _handlerName);
    }
}