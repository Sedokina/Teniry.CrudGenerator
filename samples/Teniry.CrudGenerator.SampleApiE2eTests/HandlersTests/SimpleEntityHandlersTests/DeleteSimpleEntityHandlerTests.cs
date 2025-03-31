using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.SimpleEntityFeature.DeleteSimpleEntity;
using Moq;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.SimpleEntityGenerator;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.SimpleEntityHandlersTests;

public class DeleteSimpleEntityHandlerTests {
    private readonly DeleteSimpleEntityCommand _command;
    private readonly Mock<SampleMongoDb> _db;
    private readonly DeleteSimpleEntityHandler _sut;

    public DeleteSimpleEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new(Guid.NewGuid());
    }

    [Fact]
    public async Task Should_DoNothingWhenEntityDoesNotExist() {
        // Arrange
        _db.Setup(x => x.FindAsync<SimpleEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SimpleEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new());

        // Assert
        await act.Should().NotThrowAsync();
        _db.Verify(
            x => x.FindAsync<SimpleEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Should_RemoveFromDbSetAndSave() {
        // Arrange
        _db.Setup(x => x.FindAsync<SimpleEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SimpleEntity { Id = _command.Id, Name = "Test entity" });

        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        _db.Verify(x => x.Remove(It.IsAny<SimpleEntity>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(
            x => x.FindAsync<SimpleEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }
}