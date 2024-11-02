using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.SimpleTypeEntityFeature.DeleteSimpleTypeEntity;
using ITech.CrudGenerator.TestApi.Generators.SimpleTypeEntityGenerator;
using Moq;

namespace ITech.CrudGenerator.Tests.HandlersTests.SimpleTypeEntityHandlersTests;

public class DeleteSimpleEntityHandlerTests
{
    private readonly DeleteSimpleTypeEntityCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly DeleteSimpleTypeEntityHandler _sut;

    public DeleteSimpleEntityHandlerTests()
    {
        _db = new Mock<TestMongoDb>();
        _sut = new(_db.Object);
        _command = new(Guid.NewGuid());
    }

    [Fact]
    public async Task Should_DoNothingWhenEntityDoesNotExist()
    {
        // Arrange
        _db.Setup(x => x.FindAsync<SimpleTypeEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SimpleTypeEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        await act.Should().NotThrowAsync();
        _db.Verify(x => x.FindAsync<SimpleTypeEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once);
        _db.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Should_RemoveFromDbSetAndSave()
    {
        // Arrange
        _db.Setup(x => x.FindAsync<SimpleTypeEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SimpleTypeEntity { Id = _command.Id, Name = "Test entity" });

        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        _db.Verify(x => x.Remove(It.IsAny<SimpleTypeEntity>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(x => x.FindAsync<SimpleTypeEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once);
        _db.VerifyNoOtherCalls();
    }
}