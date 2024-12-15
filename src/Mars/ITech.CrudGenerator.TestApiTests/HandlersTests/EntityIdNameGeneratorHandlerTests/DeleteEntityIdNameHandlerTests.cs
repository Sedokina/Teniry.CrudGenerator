using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.EntityIdNameFeature.DeleteEntityIdName;
using ITech.CrudGenerator.TestApi.Generators.CustomIds.EntityIdNameGenerator;
using Moq;

namespace ITech.CrudGenerator.TestApiTests.HandlersTests.EntityIdNameGeneratorHandlerTests;

public class DeleteEntityIdNameHandlerTests
{
    private readonly DeleteEntityIdNameCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly DeleteEntityIdNameHandler _sut;

    public DeleteEntityIdNameHandlerTests()
    {
        _db = new Mock<TestMongoDb>();
        _sut = new(_db.Object);
        _command = new(Guid.NewGuid());
    }

    [Fact]
    public async Task Should_DoNothingWhenEntityDoesNotExist()
    {
        // Arrange
        _db.Setup(x =>
                x.FindAsync<EntityIdName>(new object[] { _command.EntityIdNameId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((EntityIdName?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        await act.Should().NotThrowAsync();
        _db.Verify(
            x => x.FindAsync<EntityIdName>(new object[] { _command.EntityIdNameId }, It.IsAny<CancellationToken>()),
            Times.Once);
        _db.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Should_RemoveFromDbSetAndSave()
    {
        // Arrange
        _db.Setup(x =>
                x.FindAsync<EntityIdName>(new object[] { _command.EntityIdNameId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EntityIdName { EntityIdNameId = _command.EntityIdNameId, Name = "Test entity" });

        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        _db.Verify(x => x.Remove(It.IsAny<EntityIdName>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(
            x => x.FindAsync<EntityIdName>(new object[] { _command.EntityIdNameId }, It.IsAny<CancellationToken>()),
            Times.Once);
        _db.VerifyNoOtherCalls();
    }
}