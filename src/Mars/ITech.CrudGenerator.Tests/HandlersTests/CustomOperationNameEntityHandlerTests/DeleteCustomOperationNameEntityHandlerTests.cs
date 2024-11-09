using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.CustomOperationNameEntityFeature.CustomOpDeleteCustomOperationNameEntity;
using ITech.CrudGenerator.TestApi.Generators.CustomOperationNameEntityGenerator;
using ITech.CrudGenerator.Tests.E2eTests.Core;
using Moq;

namespace ITech.CrudGenerator.Tests.HandlersTests.CustomOperationNameEntityHandlerTests;

public class DeleteCustomOperationNameEntityHandlerTests
{
    private readonly CustomOpDeleteCustomOperationNameEntityCommand _command;
    private readonly CustomOpDeleteCustomOperationNameEntityHandler _sut;
    private readonly Mock<TestMongoDb> _db;

    public DeleteCustomOperationNameEntityHandlerTests()
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
                x.FindAsync<CustomOperationNameEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomOperationNameEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        await act.Should().NotThrowAsync();
        _db.Verify(
            x => x.FindAsync<CustomOperationNameEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once);
        _db.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Should_RemoveFromDbSetAndSave()
    {
        // Arrange
        _db.Setup(x =>
                x.FindAsync<CustomOperationNameEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CustomOperationNameEntity { Id = _command.Id, Name = "Test entity" });

        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        _db.Verify(x => x.Remove(It.IsAny<CustomOperationNameEntity>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(
            x => x.FindAsync<CustomOperationNameEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once);
        _db.VerifyNoOtherCalls();
    }
    
    [Theory]
    [InlineData("CustomOpDeleteCustomOperationNameEntityCommand")]
    [InlineData("CustomOpDeleteCustomOperationNameEntityHandler")]
    public void Should_BeInOperationNamespace(string typeName)
    {
        // Assert
        typeof(Program).Assembly.Should()
            .BeInNamespaceThatEndsWith(typeName, "CustomOpDeleteCustomOperationNameEntity");
    }
}