using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.CompanyFeature.CreateCompany;
using ITech.CrudGenerator.TestApi.Generators.CompanyGenerator;
using Moq;

namespace ITech.CrudGenerator.Tests.HandlersTests;

public class CreateHandlerTests
{
    private readonly CreateCompanyCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly CreateCompanyHandler _sut;

    public CreateHandlerTests()
    {
        _db = new Mock<TestMongoDb>();
        _sut = new CreateCompanyHandler(_db.Object);
        _command = new CreateCompanyCommand
        {
            Name = "My test company"
        };
    }

    [Fact]
    public async Task Should_ReturnCorrectValue()
    {
        // Arrange
        _db.Setup(x => x.AddAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
            .Callback((Company company, CancellationToken _) => company.Id = Guid.NewGuid());

        // Act
        var createdCompanyDto = await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        createdCompanyDto.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_MapCommandToEntityCorrectly()
    {
        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        _db.Verify(
            x => x.AddAsync(It.Is<Company>(c => c.Name.Equals("My test company")),
                It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task Should_AddToDbSetAndSave()
    {
        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        _db.Verify(x => x.AddAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.VerifyNoOtherCalls();
    }
}