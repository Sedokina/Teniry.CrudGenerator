using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.CustomGottenEntityFeature.CustomGottenEntityGetListOperationCustomNs;
using ITech.CrudGenerator.TestApi.Generators.CustomGottenEntity;
using Moq;
using Moq.EntityFrameworkCore;

namespace ITech.CrudGenerator.Tests.HandlersTests.CustomGottenEntityHandlerTests;

public class GetCustomGottenEntitiesListHandlerTests
{
    private readonly Mock<TestMongoDb> _db;
    private readonly CustomizedNameGetCustomEntitiesListQuery _query;
    private readonly CustomizedNameGetCustomEntitiesListHandler _sut;

    public GetCustomGottenEntitiesListHandlerTests()
    {
        _db = new Mock<TestMongoDb>();
        _sut = new(_db.Object);
        _query = new()
        {
            Name = "Test Entity",
            Sort = ["id", "name"],
            Page = 1,
            PageSize = 10
        };
    }

    [Fact]
    public async Task Should_ChangeEntityDataAndSave()
    {
        // Arrange
        _db.Setup(x => x.Set<CustomGottenEntity>())
            .ReturnsDbSet([new CustomGottenEntity { Id = Guid.NewGuid(), Name = "Test Entity" }]);

        // Act
        var entities = await _sut.HandleAsync(_query, new CancellationToken());

        // Assert
        entities.Should().BeOfType<CustomizedNameGetCustomEntitiesListDto>();
        entities.Page.Should().NotBeNull();
        entities.Page.CurrentPageIndex.Should().Be(1);
        entities.Page.PageSize.Should().Be(10);
        entities.Items.Should().SatisfyRespectively(dto =>
        {
            dto.Id.Should().NotBeEmpty();
            dto.Name.Should().NotBeEmpty();
        });
    }

    [Fact]
    public void Should_HaveCorrectSortKeys()
    {
        // Assert
        _query.GetSortKeys()
            .Should().ContainInConsecutiveOrder(["id", "name"]);
    }
    
    [Theory]
    [InlineData("CustomizedNameGetCustomEntitiesListQuery")]
    [InlineData("CustomizedNameGetCustomEntitiesListHandler")]
    [InlineData("CustomizedNameGetCustomEntitiesListDto")]
    public void Should_BeInCustomNamespace(string typeName)
    {
        // Act
        var foundTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.Name.Equals(typeName));

        // Assert
        foundTypes.Should().AllSatisfy(x =>
        {
            x.Namespace.Should().EndWith("CustomGottenEntityGetListOperationCustomNs");
        });
    }
}