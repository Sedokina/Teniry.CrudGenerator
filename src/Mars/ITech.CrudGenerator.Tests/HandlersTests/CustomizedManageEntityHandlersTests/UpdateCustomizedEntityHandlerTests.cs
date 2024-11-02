using ITech.Cqrs.Domain.Exceptions;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.CustomizedManageEntityFeature.UpdateCustomizedManageEntity;
using ITech.CrudGenerator.TestApi.Generators.CustomizedManageEntity;
using Moq;

namespace ITech.CrudGenerator.Tests.HandlersTests.CustomizedManageEntityHandlersTests;

public class UpdateCustomizedEntityHandlerTests
{
    private readonly CustomizedNameUpdateManageEntityCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly CustomizedNameUpdateManageEntityHandler _sut;

    public UpdateCustomizedEntityHandlerTests()
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
        _db.Setup(x => x.FindAsync<CustomizedManageEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomizedManageEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        await act.Should().ThrowAsync<EfEntityNotFoundException>()
            .Where(x => x.TypeName.Equals(nameof(CustomizedManageEntity)));
    }

    [Fact]
    public async Task Should_ChangeEntityDataAndSave()
    {
        // Arrange
        var entity = new CustomizedManageEntity { Id = _command.Id, Name = "Old entity name" };
        _db.Setup(x => x.FindAsync<CustomizedManageEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        entity.Name.Should().Be("New entity name");
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(
            x => x.FindAsync<CustomizedManageEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once);
        _db.VerifyNoOtherCalls();
    }
}