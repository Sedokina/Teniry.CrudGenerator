using Mars.Generators.ApplicationGenerators.Core;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Generators;

public class CreateCommandCrudGenerator : BaseCrudGenerator<BaseCommandGeneratorConfiguration>
{
    private readonly string _commandName;
    private readonly string _handlerName;
    private readonly string _endpointClassName;

    public CreateCommandCrudGenerator(
        GeneratorExecutionContext context,
        ISymbol symbol,
        BaseCommandGeneratorConfiguration configuration) : base(context, symbol, configuration)
    {
        _commandName = Configuration.CommandNameConfiguration.GetName(EntityName);
        _handlerName = Configuration.HandlerNameConfiguration.GetName(EntityName);
        _endpointClassName = Configuration.EndpointNameConfiguration.GetName(EntityName);
    }

    public override void RunGenerator()
    {
        GenerateCommand(Configuration.CommandTemplatePath);
        GenerateHandler(Configuration.HandlerTemplatePath);
        GenerateEndpoint(Configuration.EndpointTemplatePath);
    }

    private void GenerateCommand(string templatePath)
    {
        var properties = PropertiesExtractor.GetAllPropertiesOfEntity(Symbol, true);
        var model = new
        {
            CommandName = _commandName,
            Properties = properties
        };

        WriteFile(templatePath, model, _commandName);
    }

    private void GenerateHandler(string templatePath)
    {
        var model = new
        {
            CommandName = _commandName,
            HandlerName = _handlerName
        };

        WriteFile(templatePath, model, _handlerName);
    }

    private void GenerateEndpoint(string templatePath)
    {
        var model = new
        {
            EndpointClassName = _endpointClassName,
            CommandName = _commandName,
        };

        WriteFile(templatePath, model, _endpointClassName);
        EndpointMapCall = (EndpointNamespace,
            $".MapPost(\"/{EntityName.ToLower()}/create\", {_endpointClassName}.CreateAsync)");
    }
}