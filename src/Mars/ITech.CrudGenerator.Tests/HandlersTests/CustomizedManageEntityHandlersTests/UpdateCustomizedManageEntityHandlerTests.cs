using ITech.Cqrs.Domain.Exceptions;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.CustomManagedEntityFeature.ManagedEntityUpdateOperationCustomNs;
using ITech.CrudGenerator.TestApi.Generators.CustomManagedEntity;
using ITech.CrudGenerator.Tests.E2eTests.Core;
using Moq;

namespace ITech.CrudGenerator.Tests.HandlersTests.CustomizedManageEntityHandlersTests;

public class UpdateCustomizedManageEntityHandlerTests
{
    private readonly CustomizedNameUpdateManagedEntityCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly CustomizedNameUpdateManagedEntityHandler _sut;

    public UpdateCustomizedManageEntityHandlerTests()
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
        _db.Setup(x => x.FindAsync<CustomManagedEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomManagedEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        await act.Should().ThrowAsync<EfEntityNotFoundException>()
            .Where(x => x.TypeName.Equals(nameof(CustomManagedEntity)));
    }

    [Fact]
    public async Task Should_ChangeEntityDataAndSave()
    {
        // Arrange
        var entity = new CustomManagedEntity { Id = _command.Id, Name = "Old entity name" };
        _db.Setup(x => x.FindAsync<CustomManagedEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        entity.Name.Should().Be("New entity name");
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(
            x => x.FindAsync<CustomManagedEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once);
        _db.VerifyNoOtherCalls();
    }
    
    [Theory]
    [InlineData("CustomizedNameUpdateManagedEntityCommand")]
    [InlineData("CustomizedNameUpdateManagedEntityHandler")]
    public void Should_BeInCustomNamespace(string typeName)
    {
        // Assert
        typeof(Program).Assembly.Should().BeInNamespaceThatEndsWith(typeName, "ManagedEntityUpdateOperationCustomNs");
        
    }
}