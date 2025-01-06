using System.Linq;
using System.Threading;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core.SyntaxFactoryBuilders;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core.SyntaxFactoryBuilders.Models;
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
        _commandName = Scheme.Configuration.Operation;
        _handlerName = Scheme.Configuration.Handler;
        _endpointClassName = Scheme.Configuration.Endpoint.Name;
    }

    public override void RunGenerator()
    {
        GenerateCommand();
        GenerateHandler();
        if (Scheme.Configuration.Endpoint.Generate)
        {
            GenerateEndpoint();
        }
    }

    private void GenerateCommand()
    {
        var command = new ClassBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.PartialKeyword
            ], _commandName)
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation)
            .WithXmlDoc($"Delete {EntityScheme.EntityTitle}", "Nothing");

        var constructorParameters = EntityScheme.PrimaryKeys
            .Select(x => new ParameterOfMethodBuilder(x.TypeName, x.PropertyNameAsMethodParameterName)).ToList();
        var constructor = new ConstructorBuilder(_commandName)
            .WithParameters(constructorParameters);
        var constructorBody = new MethodBodyBuilder();
        foreach (var primaryKey in EntityScheme.PrimaryKeys)
        {
            command.WithProperty(primaryKey.TypeName, primaryKey.PropertyName);
            constructorBody.AssignVariable(primaryKey.PropertyName, primaryKey.PropertyNameAsMethodParameterName);
        }

        constructor.WithBody(constructorBody);
        command.WithConstructor(constructor.Build());

        WriteFile(_commandName, command.BuildAsString());
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

        var constructor = new ConstructorBuilder(_handlerName)
            .WithParameters([new ParameterOfMethodBuilder(Scheme.DbContextScheme.DbContextName, "db")]);
        var constructorBody = new MethodBodyBuilder()
            .AssignVariable("_db", "db");

        constructor.WithBody(constructorBody);

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
            .WithAttribute(new ProducesResponseTypeAttributeBuilder(204))
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

        EndpointMap = new EndpointMap(EntityScheme.EntityTitle.ToString(),
            Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature,
            "Delete",
            Scheme.Configuration.Endpoint.Route,
            _endpointClassName,
            Scheme.Configuration.Endpoint.FunctionName);
    }
}