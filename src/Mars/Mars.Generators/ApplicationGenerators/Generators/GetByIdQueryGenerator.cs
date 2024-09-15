using Mars.Generators.ApplicationGenerators.Core;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Generators;

public class GetByIdQueryGenerator : BaseGenerator
{
    private readonly string _dtoName;
    private readonly string _handlerName;
    private readonly string _queryName;

    public GetByIdQueryGenerator(
        GeneratorExecutionContext context,
        ISymbol symbol,
        CrudGeneratorConfiguration configuration)
        : base(context, symbol, configuration, configuration.GetByIdQueryGenerator.FunctionNameConfiguration)
    {
        _queryName = Configuration.GetByIdQueryGenerator.QueryNameConfiguration.GetName(_entityName);
        _dtoName = Configuration.GetByIdQueryGenerator.DtoNameConfiguration.GetName(_entityName);
        _handlerName = Configuration.GetByIdQueryGenerator.HandlerNameConfiguration.GetName(_entityName);
    }

    public void RunGenerator()
    {
        GenerateQuery(Configuration.GetByIdQueryGenerator.QueryTemplatePath);
        GenerateDto(Configuration.GetByIdQueryGenerator.DtoTemplatePath);
        GenerateHandler(Configuration.GetByIdQueryGenerator.HandlerTemplatePath);
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