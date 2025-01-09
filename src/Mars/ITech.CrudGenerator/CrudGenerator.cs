using System.Collections.Generic;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global.Factories;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;
using ITech.CrudGenerator.CrudGeneratorCore.GeneratorRunners;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.Extensions;
using Microsoft.CodeAnalysis;

namespace ITech.CrudGenerator;

[Generator]
public sealed class CrudGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // var diagnostics = context.CompilationProvider.Select((compilation, token) => Diagnostic.Create());
        // context.RegisterSourceOutput(diagnostics, static (context, diagnostic) => context.ReportDiagnostic(diagnostic));

        var generatorConfigurationsProviders = context.SyntaxProvider.CreateGeneratorConfigurationsProvider();
        var dbContextSchemeProviders = context.SyntaxProvider.CreateDbContextConfigurationsProvider();

        List<EndpointMap> endpointsMaps = [];
        var globalConfiguration = GlobalCrudGeneratorConfigurationDefaultConfigurationFactory.Construct();
        var sharedConfigurationBuilder = new CqrsOperationsSharedConfigurationBuilderFactory().Construct();

        var generatorRunnerProviders =
            generatorConfigurationsProviders
                .Combine(dbContextSchemeProviders.Collect())
                .WithTrackingName("GeneratorConfigurationWithDbContextProviders")
                .Select((tuple, _) => (
                        EntityGeneratorConfiguration: tuple.Left,
                        EntityScheme: EntitySchemeFactory.Construct(tuple.Left, tuple.Right[0]),
                        DbContextScheme: tuple.Right[0]
                    )
                )
                .WithTrackingName("EntitySchemeFactoryWithDbContextProviders")
                .SelectMany((tuple, _) =>
                {
                    var getByIdQueryGeneratorRunner = new GetByIdQueryGeneratorRunner(
                        globalConfiguration,
                        sharedConfigurationBuilder,
                        tuple.EntityGeneratorConfiguration.GetByIdOperation,
                        tuple.EntityScheme,
                        tuple.DbContextScheme
                    );
                    var getByIdRouteBuilder = getByIdQueryGeneratorRunner.Configuration.Endpoint.Generate
                        ? GetByIdQueryGeneratorRunner
                            .GetRouteConfigurationBuilder(tuple.EntityGeneratorConfiguration.GetByIdOperation)
                        : null;
                    return new IGeneratorRunner[]
                    {
                        getByIdQueryGeneratorRunner,

                        new GetListQueryGeneratorRunner(globalConfiguration,
                            sharedConfigurationBuilder,
                            tuple.EntityGeneratorConfiguration.GetListOperation,
                            tuple.EntityScheme,
                            tuple.DbContextScheme
                        ),
                        new CreateCommandGeneratorRunner(globalConfiguration,
                            sharedConfigurationBuilder,
                            tuple.EntityGeneratorConfiguration.CreateOperation,
                            tuple.EntityScheme,
                            tuple.DbContextScheme,
                            getByIdRouteBuilder,
                            getByIdQueryGeneratorRunner.Configuration.OperationName
                        ),
                        new UpdateCommandGeneratorRunner(globalConfiguration,
                            sharedConfigurationBuilder,
                            tuple.EntityGeneratorConfiguration.UpdateOperation,
                            tuple.EntityScheme,
                            tuple.DbContextScheme
                        ),
                        new DeleteCommandGeneratorRunner(globalConfiguration,
                            sharedConfigurationBuilder,
                            tuple.EntityGeneratorConfiguration.DeleteOperation,
                            tuple.EntityScheme,
                            tuple.DbContextScheme
                        )
                    };
                })
                .WithTrackingName("GeneratorRunnerProviders");

        // TODO: check if there are dbContextSchemes at all
        // TODO: add errors log

        context.RegisterSourceOutput(generatorRunnerProviders, (productionContext, generatorRunner) =>
            Execute(productionContext, generatorRunner, endpointsMaps)
        );

        context.RegisterSourceOutput(dbContextSchemeProviders.Collect(), (productionContext, _) =>
        {
            var mapEndpointsGenerator = new EndpointsMapGenerator(endpointsMaps, globalConfiguration);
            mapEndpointsGenerator.RunGenerator();
            WriteFiles(productionContext, mapEndpointsGenerator.GeneratedFiles);
        });
    }

    private static void Execute(
        SourceProductionContext context,
        IGeneratorRunner generatorRunner,
        List<EndpointMap> endpointsMaps)
    {
        var generatedFiles = generatorRunner.RunGenerator(endpointsMaps);
        WriteFiles(context, generatedFiles);
    }

    private static void WriteFiles(SourceProductionContext context, List<GeneratorResult> files)
    {
        foreach (var file in files)
        {
            context.AddSource(file.FileName, file.Source);
        }
    }
}