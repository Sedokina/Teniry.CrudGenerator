using System.Collections.Generic;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global.Factories;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.GeneratorRunners;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.DbContext;
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
        var globalConfigurationBuilder = GlobalCrudGeneratorConfigurationDefaultConfigurationFactory.Construct();
        var sharedConfigurationBuilder = new CqrsOperationsSharedConfigurationBuilderFactory().Construct();

        var generatorConfigurationsWithDbContextProviders =
            generatorConfigurationsProviders.Combine(dbContextSchemeProviders.Collect())
                .WithTrackingName("generatorConfigurationsWithDbContextProviders");

        var entitySchemeFactoryWithDbContextProviders = generatorConfigurationsWithDbContextProviders
            .Select((tuple, token) => (tuple.Left, new EntitySchemeFactory().Construct(
                    tuple.Left,
                    tuple.Right[0]), tuple.Right[0])
            ).WithTrackingName("entitySchemeFactoryWithDbContextProviders");
        var temp = entitySchemeFactoryWithDbContextProviders.SelectMany((tuple, token) =>
        {
            var getByIdQueryDefaultConfigurationBuilderFactory = new GetByIdQueryGeneratorRunner(
                globalConfigurationBuilder,
                sharedConfigurationBuilder,
                tuple.Left.GetByIdOperation,
                tuple.Item2,
                tuple.Item3
            );

            var result = new IGeneratorRunner[]
            {
                getByIdQueryDefaultConfigurationBuilderFactory,

                new GetListQueryGeneratorRunner(globalConfigurationBuilder,
                    sharedConfigurationBuilder,
                    tuple.Left.GetListOperation,
                    tuple.Item2,
                    tuple.Item3
                ),
                new CreateCommandGeneratorRunner(globalConfigurationBuilder,
                    sharedConfigurationBuilder,
                    tuple.Left.CreateOperation,
                    tuple.Item2,
                    tuple.Item3,
                    getByIdQueryDefaultConfigurationBuilderFactory.Builder.Build(tuple.Item2),
                    getByIdQueryDefaultConfigurationBuilderFactory.Builder
                ),
                new UpdateCommandGeneratorRunner(globalConfigurationBuilder,
                    sharedConfigurationBuilder,
                    tuple.Left.UpdateOperation,
                    tuple.Item2,
                    tuple.Item3
                ),
                new DeleteCommandGeneratorRunner(globalConfigurationBuilder,
                    sharedConfigurationBuilder,
                    tuple.Left.DeleteOperation,
                    tuple.Item2,
                    tuple.Item3
                )
            };
            return result;
        });

        // TODO: check if there are dbContextSchemes at all
        // TODO: add errors log

        context.RegisterSourceOutput(temp, (productionContext, configurationBuilderFactory) =>
            Execute(productionContext, configurationBuilderFactory, endpointsMaps)
        );

        context.RegisterSourceOutput(dbContextSchemeProviders.Collect(), (productionContext, _) =>
        {
            var mapEndpointsGenerator = new EndpointsMapGenerator(endpointsMaps, globalConfigurationBuilder);
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

internal class CrudGeneratorScheme<TConfiguration>
    where TConfiguration : CqrsOperationWithoutReturnValueGeneratorConfiguration
{
    public EntityScheme EntityScheme { get; set; }
    public DbContextScheme DbContextScheme { get; set; }
    public TConfiguration Configuration { get; set; }

    public CrudGeneratorScheme(EntityScheme entityScheme, DbContextScheme dbContextScheme, TConfiguration configuration)
    {
        EntityScheme = entityScheme;
        DbContextScheme = dbContextScheme;
        Configuration = configuration;
    }
}