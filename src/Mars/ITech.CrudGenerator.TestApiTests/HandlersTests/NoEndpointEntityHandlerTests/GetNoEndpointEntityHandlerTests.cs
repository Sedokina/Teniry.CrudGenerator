using ITech.Cqrs.Domain.Exceptions;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.NoEndpointEntityFeature.GetNoEndpointEntity;
using ITech.CrudGenerator.TestApi.Generators.NoEndpointEntityGenerator;
using Moq;

namespace ITech.CrudGenerator.TestApiTests.HandlersTests.NoEndpointEntityHandlerTests;

public class GetNoEndpointEntityHandlerTests {
    private readonly Mock<TestMongoDb> _db;
    private readonly GetNoEndpointEntityQuery _query;
    private readonly GetNoEndpointEntityHandler _sut;

    public GetNoEndpointEntityHandlerTests() {
        _db = new Mock<TestMongoDb>();
        _sut = new(_db.Object);
        _query = new(Guid.NewGuid());
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_GettingNotExistingEntity() {
        // Arrange
        _db.Setup(x => x.FindAsync<NoEndpointEntity>(new object[] { _query.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NoEndpointEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_query, new CancellationToken());

        // Assert
        await act.Should().ThrowAsync<EfEntityNotFoundException>()
            .Where(x => x.TypeName.Equals(nameof(NoEndpointEntity)));
    }

    [Fact]
    public async Task Should_GetEntityWithCorrectData() {
        // Arrange
        _db.Setup(x => x.FindAsync<NoEndpointEntity>(new object[] { _query.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NoEndpointEntity { Id = _query.Id, Name = "My test entity" });

        // Act
        var entity = await _sut.HandleAsync(_query, new CancellationToken());

        // Assert
        entity.Id.Should().Be(_query.Id);
        entity.Name.Should().Be("My test entity");
        _db.Verify(
            x => x.FindAsync<NoEndpointEntity>(new object[] { _query.Id }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }
}