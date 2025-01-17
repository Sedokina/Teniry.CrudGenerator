using FluentAssertions.Execution;
using ITech.CrudGenerator.Tests.Helpers;

namespace ITech.CrudGenerator.Tests;

public class IncrementalGeneratorTests
{
    [Fact]
    public void Should_TakeCachedSources_OnSecondRun()
    {
        // Arrange
        const string source = """
                              using Microsoft.EntityFrameworkCore;
                              using ITech.CrudGenerator.Abstractions.DbContext;
                              using ITech.CrudGenerator.Abstractions.Configuration;

                              namespace ITech.CrudGenerator.Tests;

                              public class TestEntity {
                                     public int Id { get; set; }
                                     public string Name { get; set; }
                              }

                              public class TestEntityGeneratorConfiguration : EntityGeneratorConfiguration<TestEntity> {}

                              [UseDbContext(DbContextDbProvider.Mongo)]
                              public class TestDb : DbContext {}
                              """;

        // Act
        var (diagnostics, output) = CrudHelper.RunGenerator(source);

        // Assert
        using var scope = new AssertionScope();
        diagnostics.Should().BeEmpty();
        output.Should().HaveCount(22);
    }
}