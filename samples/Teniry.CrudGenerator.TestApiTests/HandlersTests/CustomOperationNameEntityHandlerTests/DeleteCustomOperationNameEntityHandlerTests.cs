using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.CustomOperationNameEntityFeature.CustomOpDeleteCustomOperationNameEntity;
using Teniry.CrudGenerator.SampleApi.Generators.CustomOperationNameEntityGenerator;
using Teniry.CrudGenerator.TestApiTests.E2eTests.Core;
using Moq;

namespace Teniry.CrudGenerator.TestApiTests.HandlersTests.CustomOperationNameEntityHandlerTests;

public class DeleteCustomOperationNameEntityHandlerTests {
    private readonly CustomOpDeleteCustomOperationNameEntityCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly CustomOpDeleteCustomOperationNameEntityHandler _sut;

    public DeleteCustomOperationNameEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new(Guid.NewGuid());
    }

    [Fact]
    public async Task Should_DoNothingWhenEntityDoesNotExist() {
        // Arrange
        _db.Setup(
                x =>
                    x.FindAsync<CustomOperationNameEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((CustomOperationNameEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new());

        // Assert
        await act.Should().NotThrowAsync();
        _db.Verify(
            x => x.FindAsync<CustomOperationNameEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Should_RemoveFromDbSetAndSave() {
        // Arrange
        _db.Setup(
                x =>
                    x.FindAsync<CustomOperationNameEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new CustomOperationNameEntity { Id = _command.Id, Name = "Test entity" });

        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        _db.Verify(x => x.Remove(It.IsAny<CustomOperationNameEntity>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(
            x => x.FindAsync<CustomOperationNameEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("CustomOpDeleteCustomOperationNameEntityCommand")]
    [InlineData("CustomOpDeleteCustomOperationNameEntityHandler")]
    public void Should_BeInOperationNamespace(string typeName) {
        // Assert
        typeof(Program).Assembly.Should()
            .BeInNamespaceThatEndsWith(typeName, "CustomOpDeleteCustomOperationNameEntity");
    }
}