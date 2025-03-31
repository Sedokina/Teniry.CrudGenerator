using Teniry.Cqrs.Extended.Exceptions;
using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.GuidEntityFeature.GetGuidEntity;
using Moq;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.CustomIds.GuidEntityGenerator;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.GuidEntityGeneratorHandlerTests;

public class GetGuidEntityHandlerTests {
    private readonly Mock<SampleMongoDb> _db;
    private readonly GetGuidEntityQuery _query;
    private readonly GetGuidEntityHandler _sut;

    public GetGuidEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _query = new(Guid.NewGuid());
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_GettingNotExistingEntity() {
        // Arrange
        _db.Setup(x => x.FindAsync<GuidEntity>(new object[] { _query.GuidEntityId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GuidEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_query, new());

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>()
            .Where(x => x.NotFoundType == typeof(GuidEntity));
    }

    [Fact]
    public async Task Should_GetEntityWithCorrectData() {
        // Arrange
        _db.Setup(x => x.FindAsync<GuidEntity>(new object[] { _query.GuidEntityId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GuidEntity { GuidEntityId = _query.GuidEntityId, Name = "My test entity" });

        // Act
        var entity = await _sut.HandleAsync(_query, new());

        // Assert
        entity.GuidEntityId.Should().Be(_query.GuidEntityId);
        entity.Name.Should().Be("My test entity");
        _db.Verify(
            x => x.FindAsync<GuidEntity>(new object[] { _query.GuidEntityId }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }
}