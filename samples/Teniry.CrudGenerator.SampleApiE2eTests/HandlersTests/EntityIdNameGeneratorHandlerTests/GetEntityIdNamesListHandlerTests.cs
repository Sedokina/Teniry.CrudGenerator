using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.EntityIdNameFeature.GetEntityIdNames;
using Teniry.CrudGenerator.SampleApi.Generators.CustomIds.EntityIdNameGenerator;
using Moq;
using Moq.EntityFrameworkCore;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.EntityIdNameGeneratorHandlerTests;

public class GetEntityIdNamesListHandlerTests {
    private readonly Mock<TestMongoDb> _db;
    private readonly GetEntityIdNamesQuery _query;
    private readonly GetEntityIdNamesHandler _sut;

    public GetEntityIdNamesListHandlerTests() {
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
    public async Task Should_GetEntitiesList() {
        // Arrange
        _db.Setup(x => x.Set<EntityIdName>())
            .ReturnsDbSet([new() { EntityIdNameId = Guid.NewGuid(), Name = "Test Entity" }]);

        // Act
        var entities = await _sut.HandleAsync(_query, new());

        // Assert
        entities.Page.Should().NotBeNull();
        entities.Page.CurrentPageIndex.Should().Be(1);
        entities.Page.PageSize.Should().Be(10);
        entities.Items.Should().SatisfyRespectively(
            dto => {
                dto.EntityIdNameId.Should().NotBeEmpty();
                dto.Name.Should().NotBeEmpty();
            }
        );
    }

    [Fact]
    public void Should_HaveCorrectSortKeys() {
        // Assert
        _query.GetSortKeys()
            .Should().ContainInConsecutiveOrder("entityIdNameId", "name");
    }
}