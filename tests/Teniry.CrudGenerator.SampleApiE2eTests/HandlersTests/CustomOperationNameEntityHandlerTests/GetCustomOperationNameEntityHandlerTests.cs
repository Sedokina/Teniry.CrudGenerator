using Teniry.Cqrs.Extended.Exceptions;
using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.CustomOperationNameEntityFeature.CustomOpGetByIdCustomOperationNameEntity;
using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;
using Moq;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.CustomOperationNameEntityGenerator;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.CustomOperationNameEntityHandlerTests;

public class GetCustomOperationNameEntityHandlerTests {
    private readonly Mock<SampleMongoDb> _db;
    private readonly CustomOpGetByIdCustomOperationNameEntityQuery _query;
    private readonly CustomOpGetByIdCustomOperationNameEntityHandler _sut;

    public GetCustomOperationNameEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _query = new(Guid.NewGuid());
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_GettingNotExistingEntity() {
        // Arrange
        _db.Setup(
                x => x.FindAsync<CustomOperationNameEntity>(new object[] { _query.Id }, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((CustomOperationNameEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_query, new());

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>()
            .Where(x => x.NotFoundType == typeof(CustomOperationNameEntity));
    }

    [Fact]
    public async Task Should_GetEntityWithCorrectData() {
        // Arrange
        _db.Setup(
                x => x.FindAsync<CustomOperationNameEntity>(new object[] { _query.Id }, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new CustomOperationNameEntity { Id = _query.Id, Name = "My test entity" });

        // Act
        var entity = await _sut.HandleAsync(_query, new());

        // Assert
        entity.Id.Should().Be(_query.Id);
        entity.Name.Should().Be("My test entity");
        _db.Verify(
            x => x.FindAsync<CustomOperationNameEntity>(new object[] { _query.Id }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("CustomOpGetByIdCustomOperationNameEntityQuery")]
    [InlineData("CustomOpGetByIdCustomOperationNameEntityHandler")]
    [InlineData("CustomOperationNameEntityDto")]
    public void Should_BeInOperationNamespace(string typeName) {
        // Assert
        typeof(Program).Assembly.Should()
            .BeInNamespaceThatEndsWith(typeName, "CustomOpGetByIdCustomOperationNameEntity");
    }
}