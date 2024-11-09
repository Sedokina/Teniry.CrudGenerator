using ITech.Cqrs.Domain.Exceptions;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.IntIdEntityFeature.GetIntIdEntity;
using ITech.CrudGenerator.TestApi.Generators.IntIdEntityGenerator;
using Moq;

namespace ITech.CrudGenerator.Tests.HandlersTests.IntIdEntityHandlerTests;

public class GetIntIdEntityHandlerTests
{
    private readonly Mock<TestMongoDb> _db;
    private readonly GetIntIdEntityQuery _query;
    private readonly GetIntIdEntityHandler _sut;

    public GetIntIdEntityHandlerTests()
    {
        _db = new Mock<TestMongoDb>();
        _sut = new(_db.Object);
        _query = new(1);
    }


    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_GettingNotExistingEntity()
    {
        // Arrange
        _db.Setup(x => x.FindAsync<IntIdEntity>(new object[] { _query.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IntIdEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_query, new CancellationToken());

        // Assert
        await act.Should().ThrowAsync<EfEntityNotFoundException>()
            .Where(x => x.TypeName.Equals(nameof(IntIdEntity)));
    }

    [Fact]
    public async Task Should_GetEntityWithCorrectData()
    {
        // Arrange
        _db.Setup(x => x.FindAsync<IntIdEntity>(new object[] { _query.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IntIdEntity { Id = _query.Id, Name = "My test entity" });

        // Act
        var entity = await _sut.HandleAsync(_query, new CancellationToken());

        // Assert
        entity.Id.Should().Be(_query.Id);
        entity.Name.Should().Be("My test entity");
        _db.Verify(x => x.FindAsync<IntIdEntity>(new object[] { _query.Id }, It.IsAny<CancellationToken>()),
            Times.Once);
        _db.VerifyNoOtherCalls();
    }
}