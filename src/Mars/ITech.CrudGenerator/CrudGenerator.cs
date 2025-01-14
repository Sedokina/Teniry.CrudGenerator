using System.Collections.Generic;
using System.Linq;
using ITech.CrudGenerator.Core.Configurations.Global;
using ITech.CrudGenerator.Core.Configurations.Shared;
using ITech.CrudGenerator.Core.Generators;
using ITech.CrudGenerator.Core.Generators.Core;
using ITech.CrudGenerator.Core.Runners;
using ITech.CrudGenerator.Core.Schemes.Entity;
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
        var globalConfiguration = GlobalCrudGeneratorConfigurationFactory.Construct();
        var sharedConfigurationBuilder = new CqrsOperationsSharedConfiguratorFactory().Construct();

        var generatorRunnerProviders =
            generatorConfigurationsProviders
                .Where(x => x.Value != null)
                .Combine(dbContextSchemeProviders.Collect())
                .WithTrackingName("GeneratorConfigurationWithDbContextProviders")
                .Select((tuple, _) => (
                        EntityGeneratorConfiguration: tuple.Left.Value!,
                        EntityScheme: EntitySchemeFactory.Construct(tuple.Left.Value!, tuple.Right[0].Value),
                        DbContextScheme: tuple.Right[0].Value
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

        context.RegisterSourceOutput(dbContextSchemeProviders.Collect(), (productionContext, p) =>
        {
            var diagnostics = p.SelectMany(x =>
                x.Diagnostics.Select(c => Diagnostic.Create(c.Descriptor, null, c.Location!.FilePath)));
            foreach (var diagnostic in diagnostics)
            {
                productionContext.ReportDiagnostic(diagnostic);
            }
        });


        // TODO: check if there are dbContextSchemes at all
        // TODO: add errors log

        context.RegisterSourceOutput(generatorRunnerProviders, (productionContext, generatorRunner) =>
            Execute(productionContext, generatorRunner, endpointsMaps)
        );

        context.RegisterSourceOutput(dbContextSchemeProviders.Collect(), (productionContext, p) =>
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