using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.CustomizedManageEntityFeature.DeleteCustomizedManageEntity;
using ITech.CrudGenerator.TestApi.Generators.CustomizedManageEntity;
using Moq;

namespace ITech.CrudGenerator.Tests.HandlersTests.CustomizedManageEntityHandlersTests;

public class DeleteCustomizedManageEntityHandlerTests
{
    private readonly CustomizedNameDeleteManageEntityCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly CustomizedNameDeleteManageEntityHandler _sut;

    public DeleteCustomizedManageEntityHandlerTests()
    {
        _db = new Mock<TestMongoDb>();
        _sut = new(_db.Object);
        _command = new(Guid.NewGuid());
    }

    [Fact]
    public async Task Should_DoNothingWhenEntityDoesNotExist()
    {
        // Arrange
        _db.Setup(x => x.FindAsync<CustomizedManageEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomizedManageEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        await act.Should().NotThrowAsync();
        _db.Verify(
            x => x.FindAsync<CustomizedManageEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once);
        _db.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Should_RemoveFromDbSetAndSave()
    {
        // Arrange
        _db.Setup(x => x.FindAsync<CustomizedManageEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CustomizedManageEntity { Id = _command.Id, Name = "Test entity" });

        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        _db.Verify(x => x.Remove(It.IsAny<CustomizedManageEntity>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(
            x => x.FindAsync<CustomizedManageEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once);
        _db.VerifyNoOtherCalls();
    }
}