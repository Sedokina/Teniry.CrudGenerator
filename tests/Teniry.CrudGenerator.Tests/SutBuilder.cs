namespace Teniry.CrudGenerator.Tests;

public class SutBuilder {
    public List<string> Usings { get; set; } = [];
    public string Entity { get; set; } = "";
    public string EntityName { get; set; } = "";
    public string DbContext { get; set; } = "";
    public string GetByIdConfiguration { get; set; } = "";
    public string GetListConfiguration { get; set; } = "";
    public string CreateConfiguration { get; set; } = "";
    public string UpdateConfiguration { get; set; } = "";
    public string PatchConfiguration { get; set; } = "";
    public string DeleteConfiguration { get; set; } = "";

    public SutBuilder WithUsings(params string[] usings) {
        Usings.AddRange(usings);

        return this;
    }

    public SutBuilder WithEntity(string entityName, string entity) {
        EntityName = entityName;
        Entity = entity;

        return this;
    }

    public SutBuilder WithDbContext(string dbContext) {
        DbContext = dbContext;

        return this;
    }

    public SutBuilder WithGetByIdConfiguration(string configuration) {
        GetByIdConfiguration = configuration;

        return this;
    }

    public SutBuilder WithGetListConfiguration(string configuration) {
        GetListConfiguration = configuration;

        return this;
    }

    public SutBuilder WithCreateConfiguration(string configuration) {
        CreateConfiguration = configuration;

        return this;
    }

    public SutBuilder WithUpdateConfiguration(string configuration) {
        UpdateConfiguration = configuration;

        return this;
    }

    public SutBuilder WithPatchConfiguration(string configuration) {
        PatchConfiguration = configuration;

        return this;
    }

    public SutBuilder WithDeleteConfiguration(string configuration) {
        DeleteConfiguration = configuration;

        return this;
    }

    public string Build() {
        var usings = string.Join(Environment.NewLine, Usings.Select(u => $"using {u};"));

        return $$"""
            {{usings}}

            namespace Teniry.CrudGenerator.Tests;

            {{DbContext}}

            {{Entity}}
            public class {{EntityName}}GeneratorConfiguration : EntityGeneratorConfiguration<{{EntityName}}> {
                public {{EntityName}}GeneratorConfiguration() {
                    {{GetByIdConfiguration}}
                    {{GetListConfiguration}}
                    {{CreateConfiguration}}
                    {{UpdateConfiguration}}
                    {{PatchConfiguration}}
                    {{DeleteConfiguration}}
                }
            }
            """;
    }

    public static SutBuilder Default(bool generate = false) {
        return new SutBuilder()
            .WithUsings(
                "Microsoft.EntityFrameworkCore",
                "Teniry.CrudGenerator.Abstractions.DbContext",
                "Teniry.CrudGenerator.Abstractions.Configuration"
            ).WithDbContext(
                """
                [UseDbContext(DbContextDbProvider.Mongo)]
                public class TestDb : DbContext {}
                """
            ).WithEntity(
                "TestEntity",
                """
                public class TestEntity {
                       public int Id { get; set; }
                       public string Name { get; set; }
                }
                """
            )
            .WithGetByIdConfiguration(
                $$"""
                GetByIdOperation = new() {
                   Generate = {{generate.ToString().ToLower()}}
                };
                """
            )
            .WithGetListConfiguration(
                $$"""
                GetListOperation = new() {
                   Generate = {{generate.ToString().ToLower()}}
                };
                """
            )
            .WithCreateConfiguration(
                $$"""
                CreateOperation = new() {
                    Generate = {{generate.ToString().ToLower()}}
                };
                """
            )
            .WithUpdateConfiguration(
                $$"""
                UpdateOperation = new() {
                    Generate = {{generate.ToString().ToLower()}}
                };
                """
            )
            .WithPatchConfiguration(
                $$"""
                PatchOperation = new() {
                    Generate = {{generate.ToString().ToLower()}}
                };
                """
            )
            .WithDeleteConfiguration(
                $$"""
                DeleteOperation = new() {
                    Generate = {{generate.ToString().ToLower()}}
                };
                """
            );
    }
}