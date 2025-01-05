using System.Linq;
using System.Threading;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.Formatters;
using Microsoft.CodeAnalysis.CSharp;

namespace ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators;

internal class
    DeleteCommandCrudGenerator : BaseOperationCrudGenerator<CqrsOperationWithoutReturnValueGeneratorConfiguration>
{
    private readonly string _commandName;
    private readonly string _handlerName;
    private readonly string _endpointClassName;

    public DeleteCommandCrudGenerator(
        CrudGeneratorScheme<CqrsOperationWithoutReturnValueGeneratorConfiguration> scheme) : base(scheme)
    {
        _commandName = Scheme.Configuration.Operation.Name;
        _handlerName = Scheme.Configuration.Handler.Name;
        _endpointClassName = Scheme.Configuration.Endpoint.Name;
    }

    public override void RunGenerator()
    {
        GenerateCommand(Scheme.Configuration.Operation.TemplatePath);
        GenerateHandler();
        if (Scheme.Configuration.Endpoint.Generate)
        {
            GenerateEndpoint();
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

    private void GenerateHandler()
    {
          var handlerClass = new ClassBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.PartialKeyword
            ], _handlerName)
            .WithUsings([
                "ITech.Cqrs.Cqrs.Commands",
                Scheme.DbContextScheme.DbContextNamespace,
                EntityScheme.EntityNamespace,
            ])
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation)
            .Implements("ICommandHandler", _commandName)
            .WithPrivateField([SyntaxKind.PrivateKeyword, SyntaxKind.ReadOnlyKeyword],
                Scheme.DbContextScheme.DbContextName, "_db");

        var constructor = new ConstructorBuilder([SyntaxKind.PublicKeyword], _handlerName)
            .WithParameters([new ParameterOfMethodBuilder(Scheme.DbContextScheme.DbContextName, "db")]);
        var constructorBody = new MethodBodyBuilder()
            .AssignVariable("_db", "db");

        constructor.WithBody(constructorBody.Build());

        var methodBuilder = new MethodBuilder([
                    SyntaxKind.PublicKeyword,
                    SyntaxKind.AsyncKeyword
                ], "Task", "HandleAsync")
            .WithParameters([
                new ParameterOfMethodBuilder(_commandName, "command"),
                new ParameterOfMethodBuilder(nameof(CancellationToken), "cancellation")
            ])
            .WithXmlInheritdoc();

        var findParameters = EntityScheme.PrimaryKeys.GetAsMethodCallParameters("command");
        var methodBodyBuilder = new MethodBodyBuilder()
            .InitArrayVariable("object", "entityIds", findParameters)
            .InitVariableFromGenericAsyncMethodCall("entity", "_db", "FindAsync",
                [EntityScheme.EntityName.ToString()],
                ["entityIds", "cancellation"])
            .ReturnIfNull("entity")
            .CallMethod("_db", "Remove", ["entity"])
            .CallAsyncMethod("_db", "SaveChangesAsync", ["cancellation"]);


        methodBuilder.WithBody(methodBodyBuilder.Build());
        handlerClass.WithConstructor(constructor.Build());
        handlerClass.WithMethod(methodBuilder.Build());

        WriteFile(_handlerName, handlerClass.BuildAsString());
    }

    private void GenerateEndpoint()
    {
        var endpointClass = new ClassBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.StaticKeyword,
                SyntaxKind.PartialKeyword
            ], _endpointClassName)
            .WithUsings([
                "Microsoft.AspNetCore.Mvc",
                "ITech.Cqrs.Cqrs.Commands",
                Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation
            ])
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature);

        var methodBuilder = new MethodBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.StaticKeyword,
                SyntaxKind.AsyncKeyword
            ], "Task<IResult>", Scheme.Configuration.Endpoint.FunctionName)
            .WithParameters(EntityScheme.PrimaryKeys
                .Select(x => new ParameterOfMethodBuilder(x.TypeName, x.PropertyNameAsMethodParameterName))
                .Append(new ParameterOfMethodBuilder("ICommandDispatcher", "commandDispatcher"))
                .Append(new ParameterOfMethodBuilder("CancellationToken", "cancellation"))
                .ToList())
            .WithProducesResponseTypeAttribute(204)
            .WithXmlDoc($"Delete {Scheme.EntityScheme.EntityTitle}",
                204,
                $"{Scheme.EntityScheme.EntityTitle} deleted");

        var methodBodyBuilder = new MethodBodyBuilder()
            .InitVariableFromConstructorCall("command", _commandName, EntityScheme.PrimaryKeys)
            .CallGenericAsyncMethod("commandDispatcher", "DispatchAsync", [_commandName], ["command", "cancellation"])
            .ReturnTypedResultNoContent();

        methodBuilder.WithBody(methodBodyBuilder.Build());
        endpointClass.WithMethod(methodBuilder.Build());

        WriteFile(_endpointClassName, endpointClass.BuildAsString());

        EndpointMap = new EndpointMap(EntityScheme.EntityName.ToString(),
            Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature,
            "Delete",
            Scheme.Configuration.Endpoint.Route,
            $"{_endpointClassName}.{Scheme.Configuration.Endpoint.FunctionName}");
    }
}