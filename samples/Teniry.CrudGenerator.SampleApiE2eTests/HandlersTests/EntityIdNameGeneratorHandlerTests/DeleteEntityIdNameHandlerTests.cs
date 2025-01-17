using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.EntityIdNameFeature.DeleteEntityIdName;
using Teniry.CrudGenerator.SampleApi.Generators.CustomIds.EntityIdNameGenerator;
using Moq;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.EntityIdNameGeneratorHandlerTests;

public class DeleteEntityIdNameHandlerTests {
    private readonly DeleteEntityIdNameCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly DeleteEntityIdNameHandler _sut;

    public DeleteEntityIdNameHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new(Guid.NewGuid());
    }

    [Fact]
    public async Task Should_DoNothingWhenEntityDoesNotExist() {
        // Arrange
        _db.Setup(
                x =>
                    x.FindAsync<EntityIdName>(new object[] { _command.EntityIdNameId }, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((EntityIdName?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new());

        // Assert
        await act.Should().NotThrowAsync();
        _db.Verify(
            x => x.FindAsync<EntityIdName>(new object[] { _command.EntityIdNameId }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Should_RemoveFromDbSetAndSave() {
        // Arrange
        _db.Setup(
                x =>
                    x.FindAsync<EntityIdName>(new object[] { _command.EntityIdNameId }, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new EntityIdName { EntityIdNameId = _command.EntityIdNameId, Name = "Test entity" });

        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        _db.Verify(x => x.Remove(It.IsAny<EntityIdName>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(
            x => x.FindAsync<EntityIdName>(new object[] { _command.EntityIdNameId }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }
}