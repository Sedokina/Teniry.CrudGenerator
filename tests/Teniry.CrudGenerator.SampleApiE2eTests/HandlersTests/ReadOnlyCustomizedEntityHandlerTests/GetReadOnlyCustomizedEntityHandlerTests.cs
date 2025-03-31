using Moq;
using Teniry.Cqrs.Extended.Exceptions;
using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.ReadOnlyCustomizedEntityFeature.GetReadOnlyModelCustomNamespace;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.ReadOnlyCustomizedEntityGenerator;
using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.ReadOnlyCustomizedEntityHandlerTests;

public class GetReadOnlyCustomizedEntityHandlerTests {
    private readonly Mock<SampleMongoDb> _db;
    private readonly GetReadOnlyModelQuery _query;
    private readonly GetReadOnlyModelHandler _sut;

    public GetReadOnlyCustomizedEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _query = new(Guid.NewGuid());
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_GettingNotExistingEntity() {
        // Arrange
        _db.Setup(x => x.FindAsync<ReadOnlyCustomizedEntity>(new object[] { _query.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReadOnlyCustomizedEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_query, new());

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>()
            .Where(x => x.NotFoundType == typeof(ReadOnlyCustomizedEntity));
    }

    [Fact]
    public async Task Should_GetEntityWithCorrectData() {
        // Arrange
        _db.Setup(x => x.FindAsync<ReadOnlyCustomizedEntity>(new object[] { _query.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReadOnlyCustomizedEntity { Id = _query.Id, Name = "My test entity" });

        // Act
        var entity = await _sut.HandleAsync(_query, new());

        // Assert
        entity.Should().BeOfType<ReadOnlyModelCustomDto>();
        entity.Id.Should().Be(_query.Id);
        entity.Name.Should().Be("My test entity");
        _db.Verify(
            x => x.FindAsync<ReadOnlyCustomizedEntity>(new object[] { _query.Id }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("GetReadOnlyModelQuery")]
    [InlineData("GetReadOnlyModelHandler")]
    [InlineData("ReadOnlyModelCustomDto")]
    public void Should_BeInCustomNamespace(string typeName) {
        // Assert
        typeof(Program).Assembly.Should()
            .BeInNamespaceThatEndsWith(typeName, "GetReadOnlyModelCustomNamespace");
    }
}