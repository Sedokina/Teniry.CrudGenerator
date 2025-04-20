using Teniry.CrudGenerator.Tests.Helpers;

namespace Teniry.CrudGenerator.Tests;

public class CreateCommandCrudGeneratorTests {
    private const string Source = """
        using Microsoft.EntityFrameworkCore;
        using Teniry.CrudGenerator.Abstractions.DbContext;
        using Teniry.CrudGenerator.Abstractions.Configuration;

        namespace Teniry.CrudGenerator.Tests;

        public class TestEntity {
               public int Id { get; set; }
               public string Name { get; set; }
        }

        public class TestEntityGeneratorConfiguration : EntityGeneratorConfiguration<TestEntity> {
            public TestEntityGeneratorConfiguration() {
                CreateOperation = new() {
                    Generate = true
                };
                
                DeleteOperation = new() {
                    Generate = false
                };
                
                UpdateOperation = new() {
                    Generate = false
                };
                
                PatchOperation = new() {
                    Generate = false
                };
        
                {0}
        
                GetListOperation = new() {
                   Generate = false
                };
            }
        }

        [UseDbContext(DbContextDbProvider.Mongo)]
        public class TestDb : DbContext {}
        """;

    [Fact]
    public Task Should_ReturnLocationToGetEntityFromCreateEndpoint() {
        var getOperationConfiguration = """
            GetByIdOperation = new() {
               Generate = true
            };
            """;

        return CrudHelper.Verify(Source.Replace("{0}", getOperationConfiguration))
            .IgnoreGeneratedResult(x => !x.HintName.Equals("CreateTestEntityEndpoint.g.cs"));
    }

    [Fact]
    public Task Should_NotReturnLocationToGetEntityFromCreateEndpoint_When_GetOperationNotGenerated() {
        var getOperationConfiguration = """
            GetByIdOperation = new() {
               Generate = false
            };
            """;

        return CrudHelper.Verify(Source.Replace("{0}", getOperationConfiguration))
            .IgnoreGeneratedResult(x => !x.HintName.Equals("CreateTestEntityEndpoint.g.cs"));
    }
    
    [Fact]
    public Task Should_NotReturnLocationToGetEntityFromCreateEndpoint_When_GetOperationEndpointNotGenerated() {
        var getOperationConfiguration = """
            GetByIdOperation = new() {
               GenerateEndpoint = false
            };
            """;

        return CrudHelper.Verify(Source.Replace("{0}", getOperationConfiguration))
            .IgnoreGeneratedResult(x => !x.HintName.Equals("CreateTestEntityEndpoint.g.cs"));
    }
}