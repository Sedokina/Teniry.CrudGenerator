using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.NoEndpointEntityFeature.GetNoEndpointEntities;
using Moq;
using Moq.EntityFrameworkCore;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.NoEndpointEntityGenerator;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.NoEndpointEntityHandlerTests;

public class GetNoEndpointEntitiesListHandlerTests {
    private readonly Mock<SampleMongoDb> _db;
    private readonly GetNoEndpointEntitiesQuery _query;
    private readonly GetNoEndpointEntitiesHandler _sut;

    public GetNoEndpointEntitiesListHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _query = new() {
            Name = "Test Entity",
            Sort = ["id", "name"],
            Page = 1,
            PageSize = 10
        };
    }

    [Fact]
    public async Task Should_ChangeEntityDataAndSave() {
        // Arrange
        _db.Setup(x => x.Set<NoEndpointEntity>())
            .ReturnsDbSet([new() { Id = Guid.NewGuid(), Name = "Test Entity" }]);

        // Act
        var entities = await _sut.HandleAsync(_query, new());

        // Assert
        entities.Page.Should().NotBeNull();
        entities.Page.CurrentPageIndex.Should().Be(1);
        entities.Page.PageSize.Should().Be(10);
        entities.Items.Should().SatisfyRespectively(
            dto => {
                dto.Id.Should().NotBeEmpty();
                dto.Name.Should().NotBeEmpty();
            }
        );
    }

    [Fact]
    public void Should_HaveCorrectSortKeys() {
        // Assert
        _query.GetSortKeys()
            .Should().ContainInConsecutiveOrder("id", "name");
    }
}