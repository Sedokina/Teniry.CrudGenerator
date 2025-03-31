using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;
using Moq;
using Moq.EntityFrameworkCore;
using Teniry.CrudGenerator.SampleApi.Application.ReadOnlyCustomizedEntityFeature.CustomGottenEntityGetListOperationCustomNs;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.CustomGottenEntityGenerator;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.CustomGottenEntityHandlerTests;

public class GetCustomGottenEntitiesListHandlerTests {
    private readonly Mock<SampleMongoDb> _db;
    private readonly CustomizedNameGetCustomEntitiesListQuery _query;
    private readonly CustomizedNameGetCustomEntitiesListHandler _sut;

    public GetCustomGottenEntitiesListHandlerTests() {
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
        _db.Setup(x => x.Set<ReadOnlyCustomizedEntity>())
            .ReturnsDbSet([new() { Id = Guid.NewGuid(), Name = "Test Entity" }]);

        // Act
        var entities = await _sut.HandleAsync(_query, new());

        // Assert
        entities.Should().BeOfType<CustomizedNameGetCustomEntitiesListDto>();
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

    [Theory]
    [InlineData("CustomizedNameGetCustomEntitiesListQuery")]
    [InlineData("CustomizedNameGetCustomEntitiesListHandler")]
    [InlineData("CustomizedNameGetCustomEntitiesListDto")]
    public void Should_BeInCustomNamespace(string typeName) {
        // Assert
        typeof(Program).Assembly.Should()
            .BeInNamespaceThatEndsWith(typeName, "CustomGottenEntityGetListOperationCustomNs");
    }
}