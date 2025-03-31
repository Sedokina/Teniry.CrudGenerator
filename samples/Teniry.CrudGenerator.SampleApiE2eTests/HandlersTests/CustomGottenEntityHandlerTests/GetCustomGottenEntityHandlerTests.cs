using Teniry.Cqrs.Extended.Exceptions;
using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;
using Moq;
using Teniry.CrudGenerator.SampleApi.Application.ReadOnlyCustomizedEntityFeature.CustomGottenEntityGetOperationCustomNs;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.CustomGottenEntityGenerator;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.CustomGottenEntityHandlerTests;

public class GetCustomGottenEntityHandlerTests {
    private readonly Mock<SampleMongoDb> _db;
    private readonly CustomizedNameGetCustomEntityQuery _query;
    private readonly CustomizedNameGetCustomEntityHandler _sut;

    public GetCustomGottenEntityHandlerTests() {
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
        entity.Should().BeOfType<CustomizedNameGetCustomEntityDto>();
        entity.Id.Should().Be(_query.Id);
        entity.Name.Should().Be("My test entity");
        _db.Verify(
            x => x.FindAsync<ReadOnlyCustomizedEntity>(new object[] { _query.Id }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("CustomizedNameGetCustomEntityQuery")]
    [InlineData("CustomizedNameGetCustomEntityHandler")]
    [InlineData("CustomizedNameGetCustomEntityDto")]
    public void Should_BeInCustomNamespace(string typeName) {
        // Assert
        typeof(Program).Assembly.Should()
            .BeInNamespaceThatEndsWith(typeName, "CustomGottenEntityGetOperationCustomNs");
    }
}