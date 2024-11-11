using ITech.Cqrs.Domain.Exceptions;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.CustomOperationNameEntityFeature.CustomOpUpdateCustomOperationNameEntity;
using ITech.CrudGenerator.TestApi.Generators.CustomOperationNameEntityGenerator;
using ITech.CrudGenerator.TestApiTests.E2eTests.Core;
using Moq;

namespace ITech.CrudGenerator.TestApiTests.HandlersTests.CustomOperationNameEntityHandlerTests;

public class UpdateCustomOperationNameEntityHandlerTests
{
    private readonly Mock<TestMongoDb> _db;
    private readonly CustomOpUpdateCustomOperationNameEntityCommand _command;
    private readonly CustomOpUpdateCustomOperationNameEntityHandler _sut;

    public UpdateCustomOperationNameEntityHandlerTests()
    {
        _db = new Mock<TestMongoDb>();
        _sut = new(_db.Object);
        _command = new(Guid.NewGuid())
        {
            Name = "New entity name"
        };
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_UpdatingNotExistingEntity()
    {
        // Arrange
        _db.Setup(x =>
                x.FindAsync<CustomOperationNameEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomOperationNameEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        await act.Should().ThrowAsync<EfEntityNotFoundException>()
            .Where(x => x.TypeName.Equals(nameof(CustomOperationNameEntity)));
    }

    [Fact]
    public async Task Should_ChangeEntityDataAndSave()
    {
        // Arrange
        var entity = new CustomOperationNameEntity { Id = _command.Id, Name = "Old entity name" };
        _db.Setup(x =>
                x.FindAsync<CustomOperationNameEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        entity.Name.Should().Be("New entity name");
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(
            x => x.FindAsync<CustomOperationNameEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once);
        _db.VerifyNoOtherCalls();
    }
    
    [Theory]
    [InlineData("CustomOpUpdateCustomOperationNameEntityCommand")]
    [InlineData("CustomOpUpdateCustomOperationNameEntityHandler")]
    public void Should_BeInOperationNamespace(string typeName)
    {
        // Assert
        typeof(Program).Assembly.Should()
            .BeInNamespaceThatEndsWith(typeName, "CustomOpUpdateCustomOperationNameEntity");
    }
}