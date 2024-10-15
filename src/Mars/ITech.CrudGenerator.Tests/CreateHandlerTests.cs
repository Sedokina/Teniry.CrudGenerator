using ITech.CrudGenerator.Abstractions.Configuration;
using ITech.CrudGenerator.Abstractions.DbContext;
using ITech.CrudGenerator.Tests.Application.CompanyFeature.CreateCompany;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ITech.CrudGenerator.Tests;

public class CreateHandlerTests
{
    private readonly Mock<TestMongoDb> _db;
    private readonly CreateCompanyHandler _sut;
    private readonly CreateCompanyCommand _command;

    public CreateHandlerTests()
    {
        _db = new();
        _sut = new(_db.Object);
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

public class Company
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}

internal class CurrencyGeneratorConfiguration : EntityGeneratorConfiguration<Company>
{
}

[UseDbContext(DbContextDbProvider.Mongo)]
public class TestMongoDb : DbContext
{
}