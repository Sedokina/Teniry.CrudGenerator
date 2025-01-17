using ITech.Cqrs.Domain.Exceptions;
using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.CustomOperationNameEntityFeature.CustomOpUpdateCustomOperationNameEntity;
using Teniry.CrudGenerator.SampleApi.Generators.CustomOperationNameEntityGenerator;
using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;
using Moq;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.CustomOperationNameEntityHandlerTests;

public class UpdateCustomOperationNameEntityHandlerTests {
    private readonly CustomOpUpdateCustomOperationNameEntityCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly CustomOpUpdateCustomOperationNameEntityHandler _sut;

    public UpdateCustomOperationNameEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new(Guid.NewGuid()) {
            Name = "New entity name"
        };
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_UpdatingNotExistingEntity() {
        // Arrange
        _db.Setup(
                x =>
                    x.FindAsync<CustomOperationNameEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((CustomOperationNameEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new());

        // Assert
        await act.Should().ThrowAsync<EfEntityNotFoundException>()
            .Where(x => x.TypeName.Equals(nameof(CustomOperationNameEntity)));
    }

    [Fact]
    public async Task Should_ChangeEntityDataAndSave() {
        // Arrange
        var entity = new CustomOperationNameEntity { Id = _command.Id, Name = "Old entity name" };
        _db.Setup(
                x =>
                    x.FindAsync<CustomOperationNameEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(entity);

        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        entity.Name.Should().Be("New entity name");
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(
            x => x.FindAsync<CustomOperationNameEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("CustomOpUpdateCustomOperationNameEntityCommand")]
    [InlineData("CustomOpUpdateCustomOperationNameEntityHandler")]
    public void Should_BeInOperationNamespace(string typeName) {
        // Assert
        typeof(Program).Assembly.Should()
            .BeInNamespaceThatEndsWith(typeName, "CustomOpUpdateCustomOperationNameEntity");
    }
}