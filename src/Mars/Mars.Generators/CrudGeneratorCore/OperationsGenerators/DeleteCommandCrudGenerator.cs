using Mars.Generators.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using Mars.Generators.CrudGeneratorCore.OperationsGenerators.Core;
using Mars.Generators.CrudGeneratorCore.Schemes.Entity.Formatters;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.CrudGeneratorCore.OperationsGenerators;

internal class
    DeleteCommandCrudGenerator : BaseOperationCrudGenerator<CqrsOperationWithoutReturnValueGeneratorConfiguration>
{
    private readonly string _commandName;
    private readonly string _handlerName;
    private readonly string _endpointClassName;

    public DeleteCommandCrudGenerator(
        GeneratorExecutionContext context,
        CrudGeneratorScheme<CqrsOperationWithoutReturnValueGeneratorConfiguration> scheme) : base(context, scheme)
    {
        _commandName = Scheme.Configuration.Operation.Name;
        _handlerName = Scheme.Configuration.Handler.Name;
        _endpointClassName = Scheme.Configuration.Endpoint.Name;
    }

    public override void RunGenerator()
    {
        GenerateCommand(Scheme.Configuration.Operation.TemplatePath);
        GenerateHandler(Scheme.Configuration.Handler.TemplatePath);
        if (Scheme.Configuration.Endpoint.Generate)
        {
            GenerateEndpoint(Scheme.Configuration.Endpoint.TemplatePath);
        }
    }

    private void GenerateCommand(string templatePath)
    {
        var properties = EntityScheme.PrimaryKeys.FormatAsProperties();
        var constructorParameters = EntityScheme.PrimaryKeys.FormatAsMethodDeclarationParameters();
        var constructorBody = EntityScheme.PrimaryKeys.FormatAsConstructorBody();
        var model = new
        {
            CommandName = _commandName,
            Properties = properties,
            ConstructorParameters = constructorParameters,
            ConstructorBody = constructorBody
        };
        WriteFile(templatePath, model, _commandName);
    }

    private void GenerateHandler(string templatePath)
    {
        var findParameters = EntityScheme.PrimaryKeys.FormatAsMethodCallParameters("command");
        var model = new
        {
            CommandName = _commandName,
            HandlerName = _handlerName,
            FindParameters = findParameters
        };

        WriteFile(templatePath, model, _handlerName);
    }

    private void GenerateEndpoint(string templatePath)
    {
        var routeParams = EntityScheme.PrimaryKeys.FormatAsMethodDeclarationParameters();
        var constructorParameters = EntityScheme.PrimaryKeys.FormatAsMethodCallParameters();

        var model = new
        {
            EndpointClassName = _endpointClassName,
            FunctionName = Scheme.Configuration.Endpoint.FunctionName,
            RouteParams = routeParams,
            CommandName = _commandName,
            CommandConstructorParameters = constructorParameters
        };

        WriteFile(templatePath, model, _endpointClassName);

        EndpointMap = new EndpointMap(EntityScheme.EntityName.ToString(),
            Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature,
            "Delete",
            Scheme.Configuration.Endpoint.Route,
            $"{_endpointClassName}.{Scheme.Configuration.Endpoint.FunctionName}");
    }
}